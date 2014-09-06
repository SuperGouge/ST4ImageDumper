using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace JDP {
    public class ImageDumper {
        private static readonly ResponseClassifier[] _responseClassifiers = new[] {
            new ResponseClassifier("mistyped the captcha", PostResult.IncorrectCaptcha),
            new ResponseClassifier("captcha is no longer valid", PostResult.IncorrectCaptcha),
            new ResponseClassifier("4chan pass is currently in use by another", PostResult.ChanPassNotAccepted),
            new ResponseClassifier("must wait longer", PostResult.FloodDetected),
            new ResponseClassifier("duplicate file", PostResult.DuplicateFile),
            new ResponseClassifier("upload failed", PostResult.FileNotAllowed),
            new ResponseClassifier("resolution is too large", PostResult.FileNotAllowed),
            new ResponseClassifier("file too large", PostResult.FileTooLarge),
            new ResponseClassifier("cannot find record", PostResult.CorruptFile),
            new ResponseClassifier("contains an embedded file", PostResult.ContainsEmbeddedFile),
            new ResponseClassifier("possible malicious code", PostResult.MaliciousFile),
            new ResponseClassifier("field too long", PostResult.FieldTooLong),
            new ResponseClassifier("replies has been reached", PostResult.ThreadFull),
            new ResponseClassifier("thread specified does not exist", PostResult.ThreadDead),
            new ResponseClassifier("banned", PostResult.Banned),
            new ResponseClassifier("blocked due to abuse", PostResult.Blocked),
            new ResponseClassifier("copyright claim", PostResult.Copyrighted)
        };

        static ImageDumper() {
            // Ignore invalid certificates (workaround for Mono)
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, errors) => true;
        }

        private Thread _dumpThread;
        private object _settingsSync;
        private object _eventSync;
        private bool _isRunning;
        private bool _isPaused;
        private bool _isStopping;
        private bool _isRequestingCaptchaChallenge;
        private string _imageDirectory;
        private string _threadURL;
        private string _userName;
        private string _emailAddress;
        private string _password;
        private string _chanPassToken;
        private string _chanPassPIN;
        private TimeSpan _postInterval;
        private bool _movePostedToSubdirectory;
        private bool _randomizeOrder;
        private bool _numberPosts;
        private int _postNumberStart;
        private string _nextPostComment;
        private string _captchaPublicKey;
        private List<Captcha> _captchas;
        private int _neededCaptchaCount;
        private bool _currentPostAcquiredCaptcha;
        private long _floodWaitEndTicks;
        private TimeSpan _averagePostDuration;

        // Notes:
        // * Public members should not fire events or otherwise lock on the event sync
        //   from the current thread.  This could cause a deadlock especially if executing
        //   inside an Invoke.
        // * Events should not be fired from inside a lock on the settings sync (potential
        //   deadlock).
        // * If locking on both the event and settings sync, lock on the event sync first
        //   to avoid a deadlock.

        public ImageDumper() {
            _settingsSync = new object();
            _eventSync = new object();
            _captchas = new List<Captcha>();
            _postInterval = TimeSpan.FromSeconds(30);
            _postNumberStart = 1;
        }

        public string ImageDirectory {
            get { lock (_settingsSync) { return _imageDirectory; } }
            set { SetSetting(out _imageDirectory, value); }
        }

        public string ThreadURL {
            get { lock (_settingsSync) { return _threadURL; } }
            set { SetSetting(out _threadURL, value); }
        }

        public string UserName {
            get { lock (_settingsSync) { return _userName; } }
            set { SetSetting(out _userName, value); }
        }

        public string EmailAddress {
            get { lock (_settingsSync) { return _emailAddress; } }
            set { SetSetting(out _emailAddress, value); }
        }

        public string Password {
            get { lock (_settingsSync) { return _password; } }
            set { SetSetting(out _password, value); }
        }

        public string ChanPassToken {
            get { lock (_settingsSync) { return _chanPassToken; } }
            set { SetSetting(out _chanPassToken, value); }
        }

        public string ChanPassPIN {
            get { lock (_settingsSync) { return _chanPassPIN; } }
            set { SetSetting(out _chanPassPIN, value); }
        }

        public bool UsingChanPass {
            get { lock (_settingsSync) { return !String.IsNullOrEmpty(_chanPassToken) && !String.IsNullOrEmpty(_chanPassPIN); } }
        }

        public TimeSpan PostInterval {
            get { lock (_settingsSync) { return _postInterval; } }
            set { SetSetting(out _postInterval, value); }
        }

        public bool MovePostedToSubdirectory {
            get { lock (_settingsSync) { return _movePostedToSubdirectory; } }
            set { SetSetting(out _movePostedToSubdirectory, value); }
        }

        public bool RandomizeOrder {
            get { lock (_settingsSync) { return _randomizeOrder; } }
            set { SetSetting(out _randomizeOrder, value); }
        }

        public bool NumberPosts {
            get { lock (_settingsSync) { return _numberPosts; } }
            set { SetSetting(out _numberPosts, value); }
        }

        public int PostNumberStart {
            get { lock (_settingsSync) { return _postNumberStart; } }
            set { SetSetting(out _postNumberStart, value); }
        }

        public string NextPostComment {
            get { lock (_settingsSync) { return _nextPostComment; } }
            set { lock (_settingsSync) { _nextPostComment = value; } }
        }

        public bool IsPaused {
            get { lock (_settingsSync) { return _isPaused; } }
            set {
                lock (_settingsSync) {
                    _isPaused = value;
                    Monitor.PulseAll(_settingsSync);
                }
            }
        }

        public bool IsStopping {
            get { lock (_settingsSync) { return _isStopping; } }
        }

        public bool IsRunning {
            get { lock (_settingsSync) { return _isRunning; } }
        }

        public int CachedCaptchaCount {
            get { lock (_settingsSync) { return _captchas.Count; } }
        }

        public bool CurrentPostAcquiredCaptcha {
            get { lock (_settingsSync) { return _currentPostAcquiredCaptcha; } }
        }

        public TimeSpan FloodWaitRemainingDuration {
            get { lock (_settingsSync) { return TimeSpan.FromMilliseconds(Math.Max(_floodWaitEndTicks - TickCount.Now, 0)); } }
        }

        public TimeSpan AveragePostDuration {
            get { lock (_settingsSync) { return _averagePostDuration; } }
        }

        public TimeSpan CaptchaExpirationThreshold {
            get { return TimeSpan.FromSeconds(15); }
        }

        private void SetSetting<T>(out T field, T value) {
            lock (_settingsSync) {
                if (_isRunning) {
                    throw new Exception("Setting cannot be changed while the dumper is running.");
                }
                field = value;
            }
        }

        public void AtomicSettingsAction(Action action) {
            lock (_settingsSync) {
                action();
            }
        }

        public void Start() {
            lock (_settingsSync) {
                if (_isRunning) {
                    throw new Exception("The image dumper is already running.");
                }
                _isRunning = true;
            }
            try {
                _dumpThread = new Thread(DumpThread);
                _dumpThread.IsBackground = true;
                _dumpThread.Start();
            }
            catch {
                lock (_settingsSync) {
                    _isRunning = false;
                }
                throw;
            }
        }

        public void Stop() {
            lock (_settingsSync) {
                if (!_isRunning) return;
                _isStopping = true;
                Monitor.PulseAll(_settingsSync);
            }
        }

        public void Abort() {
            ThreadPool.QueueUserWorkItem((s) => {
                lock (_settingsSync) {
                    if (!_isRunning) return;
                }
                _dumpThread.Abort();
                CleanupAfterStop();
                OnEvent(Stopped, new StoppedEventArgs(null));
            });
        }

        public void AddCaptcha(Captcha captcha) {
            ThreadPool.QueueUserWorkItem((s) => {
                captcha = ValidateCaptcha(captcha);
                if (captcha == null) return;
                lock (_settingsSync) {
                    _captchas.Add(captcha);
                    _neededCaptchaCount--;
                    Monitor.PulseAll(_settingsSync);
                }
                OnCaptchaCountChanged();
            });
        }

        private void CleanupAfterStop() {
            lock (_eventSync)
                lock (_settingsSync) {
                    _isRunning = false;
                    _isPaused = false;
                    _isStopping = false;
                    _captchaPublicKey = null;
                    _neededCaptchaCount = 0;
                    _currentPostAcquiredCaptcha = false;
                    _floodWaitEndTicks = 0;
                    _averagePostDuration = TimeSpan.Zero;
                }
        }

        private void DumpThread() {
            Exception exceptionTriggeringStop = null;

            try {
                string postedFile = Path.Combine(_imageDirectory, "_posted_.txt");
                HashSet<string> postedSet = null;
                if (File.Exists(postedFile)) {
                    postedSet = new HashSet<string>(File.ReadAllLines(postedFile));
                }

                List<string> paths =
                    (from p in Directory.GetFiles(_imageDirectory)
                        let allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webm" }
                        where allowedExtensions.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase) &&
                              (postedSet == null || !postedSet.Contains(Path.GetFileName(p)))
                        select p)
                        .OrderBy(p => Path.GetFileName(p), new CustomComparer<string>(General.CompareNatural))
                        .ToList();
                if (paths.Count == 0) {
                    throw new Exception("Folder doesn't contain any postable files.");
                }
                if (_randomizeOrder) {
                    Random random = new Random();
                    paths =
                        (from p in paths
                            select new {
                                Index = random.Next(Int32.MaxValue),
                                Path = p
                            }).OrderBy(n => n.Index).Select(n => n.Path).ToList();
                }

                bool usingChanPass = !String.IsNullOrEmpty(_chanPassToken) && !String.IsNullOrEmpty(_chanPassPIN);
                TimeSpan? overridePostInterval = TimeSpan.Zero;
                string postedDirectory = Path.Combine(_imageDirectory, "Posted");
                int originalPostNumberStart = _postNumberStart;
                int iFile = 0;
                int successfulPostCount = 0;
                TimeSpan successfulPostDuration = TimeSpan.Zero;
                int consecutiveUnhandledExceptionCount = 0;
                int consecutiveUnrecognizedResponseCount = 0;
                int consecutiveIncorrectCaptchaCount = 0;

                Action<bool, bool> afterPost = (markAsUploaded, advanceFile) => {
                    if (markAsUploaded) {
                        string pathSrc = paths[iFile];
                        if (_movePostedToSubdirectory) {
                            string pathDst = General.GetAvailableFilePath(postedDirectory, Path.GetFileName(pathSrc));
                            if (!Directory.Exists(postedDirectory)) {
                                Directory.CreateDirectory(postedDirectory);
                            }
                            General.TryMoveFile(pathSrc, pathDst, 3, TimeSpan.FromMilliseconds(500));
                        }
                        else {
                            File.AppendAllText(postedFile, Path.GetFileName(pathSrc) + Environment.NewLine);
                        }
                    }
                    if (advanceFile) {
                        iFile++;
                    }
                    if (!usingChanPass) {
                        if (_currentPostAcquiredCaptcha && !advanceFile) {
                            lock (_settingsSync) {
                                _neededCaptchaCount++;
                            }
                            OnCaptchaCountChanged();
                        }
                    }
                };

                HTMLParser pageParser = new HTMLParser(General.TextFromURL(_threadURL, Encoding.UTF8));

                HTMLTagRange formTagRange = pageParser.CreateTagRange(pageParser.FindStartTags("form").SingleOrDefault(
                    t => t.GetAttributeValueOrEmpty("name").Equals("post", StringComparison.OrdinalIgnoreCase)));
                if (formTagRange == null) {
                    throw new Exception("Unable to find post form.");
                }

                string formAction = General.GetAbsoluteURL(_threadURL, formTagRange.StartTag.GetAttributeValueOrEmpty("action"));

                FormData formData = new MultipartFormData();
                formData.FormFields.AddRange(General.GetFormFields(pageParser, formTagRange));

                long maxFileSize = Int64.Parse(formData.FormFields.GetTextField("MAX_FILE_SIZE").Text);
                paths.RemoveAll(p => new FileInfo(p).Length > maxFileSize);
                if (paths.Count == 0) {
                    throw new Exception("Folder doesn't contain any postable files.");
                }

                if (!usingChanPass) {
                    lock (_settingsSync) {
                        _captchaPublicKey =
                            (from tag in pageParser.FindStartTags("iframe")
                                let srcURI = General.GetAbsoluteURI(_threadURL, tag.GetAttributeValueOrEmpty("src"))
                                where srcURI != null &&
                                      srcURI.AbsolutePath.EndsWith("/noscript", StringComparison.Ordinal)
                                let queryParams = srcURI.Query.StartsWith("?", StringComparison.Ordinal) ? srcURI.Query.Substring(1).Split('&') : new string[0]
                                let kParam = queryParams.FirstOrDefault(p => p.StartsWith("k=", StringComparison.Ordinal))
                                where kParam != null
                                select kParam.Substring(2)).FirstOrDefault();
                        if (_captchaPublicKey == null) {
                            throw new Exception("Unable to locate the captcha public key.");
                        }
                        _captchas.RemoveAll(c => !c.PublicKey.Equals(_captchaPublicKey, StringComparison.Ordinal) || c.IsExpired(CaptchaExpirationThreshold));
                        _neededCaptchaCount = paths.Count - _captchas.Count;
                    }
                    OnEvent(CaptchaKeyFound, EventArgs.Empty);
                    OnCaptchaCountChanged();
                }

                CookieContainer cookies = new CookieContainer();
                if (usingChanPass) {
                    string authURL = String.Format("https://{0}/auth", new Uri(formAction).Host);
                    HTMLParser authPageParser = new HTMLParser(General.TextFromURL(authURL, Encoding.UTF8));
                    HTMLTagRange authFormTagRange = authPageParser.CreateTagRange(authPageParser.FindStartTag("form"));
                    if (authFormTagRange == null) {
                        throw new Exception("Unable to find auth form.");
                    }
                    string authFormAction = General.GetAbsoluteURL(authURL, authFormTagRange.StartTag.GetAttributeValueOrEmpty("action"));
                    FormData authFormData = new URLEncodedFormData();
                    authFormData.FormFields.AddRange(General.GetFormFields(authPageParser, authFormTagRange));
                    authFormData.FormFields.GetTextField("id").Text = _chanPassToken;
                    authFormData.FormFields.GetTextField("pin").Text = _chanPassPIN;
                    string authResponseText = General.PostForm(authFormAction, authURL, cookies, authFormData, Encoding.UTF8);
                    if (authResponseText.IndexOf("Your device is now authorized.", StringComparison.OrdinalIgnoreCase) == -1) {
                        throw new Exception("4chan Pass authentication failed.");
                    }
                }

                while (iFile < paths.Count) {
                    try {
                        lock (_settingsSync) {
                            _postNumberStart = originalPostNumberStart + iFile;
                            if (_isStopping) throw new StopException();
                            _floodWaitEndTicks = TickCount.Now + (int)(overridePostInterval ?? _postInterval).TotalMilliseconds;
                            _currentPostAcquiredCaptcha = false;
                        }
                        OnEvent(PostBegin, new PostBeginEventArgs(iFile, paths.Count, paths[iFile]));
                        overridePostInterval = null;

                        // Wait to post to prevent flood
                        if (TickCount.Now < _floodWaitEndTicks) {
                            OnEvent(PostStatus, new PostStatusEventArgs(PostState.FloodWait));
                            int remainingDelayMilliseconds;
                            while ((remainingDelayMilliseconds = (int)(_floodWaitEndTicks - TickCount.Now)) > 0) {
                                lock (_settingsSync) {
                                    if (_isStopping) throw new StopException();
                                    Monitor.Wait(_settingsSync, remainingDelayMilliseconds);
                                }
                            }
                        }

                        // Get a captcha and pause if requested
                        Captcha captcha = null;
                        do {
                            if (!usingChanPass) {
                                bool waitForCaptcha;
                                lock (_settingsSync) {
                                    waitForCaptcha = _captchas.Count == 0 && !_isPaused;
                                }
                                if (waitForCaptcha) {
                                    OnEvent(PostStatus, new PostStatusEventArgs(PostState.CaptchaWait));
                                    lock (_settingsSync) {
                                        while (_captchas.Count == 0 && !_isPaused) {
                                            if (_isStopping) throw new StopException();
                                            Monitor.Wait(_settingsSync);
                                        }
                                    }
                                }
                            }

                            bool isPaused;
                            lock (_settingsSync) {
                                isPaused = _isPaused;
                            }
                            if (isPaused) {
                                OnEvent(PostStatus, new PostStatusEventArgs(PostState.Paused));
                                lock (_settingsSync) {
                                    while (_isPaused) {
                                        if (_isStopping) throw new StopException();
                                        Monitor.Wait(_settingsSync);
                                    }
                                }
                            }

                            bool handledCaptcha = false;
                            lock (_settingsSync) {
                                if (_isStopping) throw new StopException();
                                if (!usingChanPass) {
                                    if (_captchas.Count != 0) {
                                        captcha = _captchas[0];
                                        _captchas.RemoveAt(0);
                                        if (captcha.IsExpired(CaptchaExpirationThreshold + _averagePostDuration)) {
                                            captcha = null;
                                            _neededCaptchaCount++;
                                        }
                                        handledCaptcha = true;
                                    }
                                }
                                _currentPostAcquiredCaptcha = usingChanPass || captcha != null;
                            }
                            if (handledCaptcha) {
                                OnCaptchaCountChanged();
                            }
                        } while (!_currentPostAcquiredCaptcha);

                        // Make sure the file still exists
                        if (!File.Exists(paths[iFile])) {
                            OnEvent(PostEnd, new PostEndEventArgs(PostResult.FileNotFound));
                            if (!usingChanPass) {
                                lock (_settingsSync) {
                                    _captchas.Insert(0, captcha);
                                    _neededCaptchaCount--;
                                    _currentPostAcquiredCaptcha = false;
                                }
                                OnCaptchaCountChanged();
                            }
                            afterPost(false, true);
                            overridePostInterval = TimeSpan.Zero;
                            continue;
                        }

                        OnEvent(PostStatus, new PostStatusEventArgs(PostState.Posting));

                        string postComment = !_numberPosts ? String.Empty :
                            String.Format("{0}/{1}", iFile + originalPostNumberStart, paths.Count + (originalPostNumberStart - 1));
                        lock (_settingsSync) {
                            if (!String.IsNullOrEmpty(_nextPostComment)) {
                                postComment = postComment.Length == 0 ? _nextPostComment : (postComment + "\r\n\r\n" + _nextPostComment);
                            }
                        }

                        // Fill in form fields
                        formData.FormFields.GetTextField("name").Text = _userName;
                        formData.FormFields.GetTextField("email").Text = _emailAddress;
                        formData.FormFields.GetTextField("pwd").Text = _password;
                        formData.FormFields.GetFileField("upfile").FilePath = paths[iFile];
                        formData.FormFields.GetTextField("com").Text = postComment;
                        formData.FormFields.GetTextField("recaptcha_challenge_field").Text = !usingChanPass ? captcha.Challenge : String.Empty;
                        formData.FormFields.GetTextField("recaptcha_challenge_field").Disabled = usingChanPass;
                        formData.FormFields.GetTextField("recaptcha_response_field").Text = !usingChanPass ? captcha.Response : String.Empty;
                        formData.FormFields.GetTextField("recaptcha_response_field").Disabled = usingChanPass;

                        // Post form and get response
                        long postStartTicks = TickCount.Now;
                        string responseHTML = General.PostForm(formAction, _threadURL, cookies, formData, Encoding.UTF8);
                        long postDurationTicks = TickCount.Now - postStartTicks;

                        // Analyze response
                        if (responseHTML.IndexOf("post successful", StringComparison.OrdinalIgnoreCase) != -1) {
                            successfulPostCount++;
                            successfulPostDuration += TimeSpan.FromMilliseconds(postDurationTicks);
                            lock (_settingsSync) {
                                _nextPostComment = null;
                                _averagePostDuration = TimeSpan.FromTicks(successfulPostDuration.Ticks / successfulPostCount);
                            }
                            OnEvent(PostEnd, new PostEndEventArgs(PostResult.Success));
                            afterPost(true, true);
                            consecutiveUnhandledExceptionCount = 0;
                            consecutiveUnrecognizedResponseCount = 0;
                            consecutiveIncorrectCaptchaCount = 0;
                        }
                        else {
                            ResponseClassifier matchingClassifier = _responseClassifiers.FirstOrDefault(c => c.IsMatch(responseHTML));
                            PostResult result = matchingClassifier != null ? matchingClassifier.ResponseResult : PostResult.UnrecognizedResponse;

                            OnEvent(PostEnd, new PostEndEventArgs(result, responseHTML, null));

                            if (result == PostResult.UnrecognizedResponse) {
                                consecutiveUnrecognizedResponseCount++;
                                if (consecutiveUnrecognizedResponseCount >= 2) {
                                    throw new FatalPostException("Detected 2 unrecognized responses in a row.");
                                }
                                afterPost(false, true);
                                overridePostInterval = TimeSpan.FromSeconds(30);
                            }
                            else if (result == PostResult.FieldTooLong) {
                                throw new FatalPostException("One of the form fields is too long.");
                            }
                            else if (result == PostResult.ThreadFull) {
                                throw new FatalPostException("The thread has reached the image limit.");
                            }
                            else if (result == PostResult.ThreadDead) {
                                throw new FatalPostException("The thread no longer exists.");
                            }
                            else if (result == PostResult.ChanPassNotAccepted) {
                                throw new FatalPostException("4chan Pass was not accepted.");
                            }
                            else if (result == PostResult.Banned) {
                                throw new FatalPostException("You are banned.");
                            }
                            else if (result == PostResult.Blocked) {
                                throw new FatalPostException("Posting from your ISP, IP range, or country has been blocked due to abuse. 4chan Pass users can bypass this block.");
                            }
                            else if (result == PostResult.IncorrectCaptcha) {
                                consecutiveIncorrectCaptchaCount++;
                                if (consecutiveIncorrectCaptchaCount >= 3) {
                                    throw new FatalPostException("Detected 3 incorrect captchas in a row.");
                                }
                                afterPost(false, false);
                                overridePostInterval = consecutiveIncorrectCaptchaCount == 1 ? TimeSpan.FromSeconds(5) : TimeSpan.FromSeconds(10);
                            }
                            else if (result == PostResult.FloodDetected) {
                                afterPost(false, false);
                                overridePostInterval = TimeSpan.FromSeconds(30);
                            }
                            else {
                                afterPost(true, true);
                                overridePostInterval = TimeSpan.FromSeconds(5);
                            }
                            consecutiveUnhandledExceptionCount = 0;
                            if (result != PostResult.UnrecognizedResponse) consecutiveUnrecognizedResponseCount = 0;
                            if (result != PostResult.IncorrectCaptcha) consecutiveIncorrectCaptchaCount = 0;
                        }
                    }
                    catch (StopException) {
                        throw;
                    }
                    catch (ThreadAbortException) {
                        throw;
                    }
                    catch (FatalPostException) {
                        throw;
                    }
                    catch (Exception ex) {
                        OnEvent(PostEnd, new PostEndEventArgs(PostResult.UnhandledException, null, ex));
                        consecutiveUnhandledExceptionCount++;
                        if (consecutiveUnhandledExceptionCount >= 2) {
                            throw new FatalPostException("Detected 2 unhandled exceptions in a row.");
                        }
                        afterPost(false, false);
                        overridePostInterval = TimeSpan.FromSeconds(30);
                        consecutiveUnrecognizedResponseCount = 0;
                        consecutiveIncorrectCaptchaCount = 0;
                    }
                }

                // Posted all files, reset numbering
                lock (_settingsSync) {
                    _postNumberStart = 1;
                }
            }
            catch (StopException) { }
            catch (ThreadAbortException) { }
            catch (Exception ex) {
                exceptionTriggeringStop = ex;
            }

            CleanupAfterStop();
            OnEvent(Stopped, new StoppedEventArgs(exceptionTriggeringStop));
        }

        public void LoadCaptchasFromFile(string path) {
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path);

            lock (_settingsSync) {
                foreach (string line in lines) {
                    string[] s = line.Split('\t');
                    if (s.Length != 5) continue;

                    DateTime timeAcquiredUTC;
                    if (!DateTime.TryParseExact(s[1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out timeAcquiredUTC)) 
                    {
                        continue;
                    }

                    int timeoutSeconds;
                    if (!Int32.TryParse(s[2], out timeoutSeconds)) continue;

                    if (s[1].Length == 0 || s[2].Length == 0) continue;

                    _captchas.Add(new Captcha {
                        PublicKey = s[0],
                        TimeAcquiredUTC = timeAcquiredUTC,
                        TimeoutSeconds = timeoutSeconds,
                        Challenge = s[3],
                        Response = s[4]
                    });
                }
            }
        }

        public void SaveCaptchasToFile(string path) {
            string[] lines;
            lock (_settingsSync) {
                lines = _captchas.Select(c => String.Format("{0}\t{1:yyyyMMddHHmmss}\t{2}\t{3}\t{4}",
                    c.PublicKey, c.TimeAcquiredUTC, c.TimeoutSeconds, c.Challenge, c.Response)).ToArray();
            }
            File.WriteAllLines(path, lines);
        }

        public void RequestCaptchaChallenge() {
            lock (_settingsSync) {
                if (_isRequestingCaptchaChallenge) {
                    throw new Exception("A captcha challenge request is already in progress.");
                }
                _isRequestingCaptchaChallenge = true;
            }
            try {
                ThreadPool.QueueUserWorkItem((s) => RequestCaptchaChallengeThread());
            }
            catch {
                lock (_settingsSync) {
                    _isRequestingCaptchaChallenge = false;
                }
                throw;
            }
        }

        private void RequestCaptchaChallengeThread() {
            try {
                string captchaPublicKey = GetCaptchaPublicKey();
                DateTime timeAcquiredUTC = DateTime.UtcNow;
                string scriptURL = "http://www.google.com/recaptcha/api/challenge?k=" + captchaPublicKey;
                string scriptText = General.TextFromURL(scriptURL, Encoding.UTF8);
                string[] scriptLines = scriptText.Replace("\r\n", "\n").Replace("\r", "\n")
                    .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, string> variables = new Dictionary<string, string>();
                bool foundOpenBrace = false;
                foreach (string line in scriptLines) {
                    if (!foundOpenBrace) {
                        foundOpenBrace = line.TrimEnd().EndsWith("{", StringComparison.Ordinal);
                    }
                    else if (line.TrimStart().StartsWith("}", StringComparison.Ordinal)) {
                        break;
                    }
                    else {
                        int pos = line.IndexOf(":", StringComparison.Ordinal);
                        if (pos == -1) continue;
                        string name = line.Substring(0, pos).Trim();
                        string value = line.Substring(pos + 1).Trim();
                        if (value.EndsWith(",", StringComparison.Ordinal)) {
                            value = value.Substring(0, value.Length - 1).TrimEnd();
                        }
                        if (value.StartsWith("'", StringComparison.Ordinal) && value.EndsWith("'", StringComparison.Ordinal)) {
                            value = value.Substring(1, value.Length - 2);
                        }
                        variables[name] = value;
                    }
                }

                string challenge = variables["challenge"];
                int timeoutSeconds = Int32.Parse(variables["timeout"]);

                Image challengeImage = General.ImageFromURL("http://www.google.com/recaptcha/api/image?c=" + challenge);

                Captcha captcha = new Captcha {
                    PublicKey = captchaPublicKey,
                    TimeAcquiredUTC = timeAcquiredUTC,
                    TimeoutSeconds = timeoutSeconds,
                    Challenge = challenge
                };

                OnEvent(CaptchaChallengeReceived, new CaptchaChallengeReceivedEventArgs(challengeImage, captcha));
            }
            catch {
                OnEvent(CaptchaChallengeReceiveError, EventArgs.Empty);
            }
            lock (_settingsSync) {
                _isRequestingCaptchaChallenge = false;
            }
        }

        private Captcha ValidateCaptcha(Captcha captcha) {
            try {
                string pageURL = String.Format(
                    "http://www.google.com/recaptcha/api/noscript?k={0}&recaptcha_challenge_field={1}&recaptcha_response_field={2}",
                    GetCaptchaPublicKey(), captcha.Challenge, Uri.EscapeDataString(captcha.Response));
                HTMLParser pageParser = new HTMLParser(General.TextFromURL(pageURL, Encoding.UTF8));

                HTMLTagRange textAreaTagRange = pageParser.CreateTagRange(pageParser.FindStartTag("textarea"));
                if (textAreaTagRange != null) {
                    return new Captcha {
                        PublicKey = captcha.PublicKey,
                        TimeAcquiredUTC = captcha.TimeAcquiredUTC,
                        TimeoutSeconds = captcha.TimeoutSeconds,
                        Challenge = pageParser.GetInnerHTML(textAreaTagRange),
                        Response = "manual_challenge"
                    };
                }
            }
            catch { }

            return null;
        }

        private string GetCaptchaPublicKey() {
            string captchaPublicKey;
            lock (_settingsSync) {
                captchaPublicKey = _captchaPublicKey;
            }
            if (captchaPublicKey == null) {
                throw new Exception("Captcha public key not loaded.");
            }
            return captchaPublicKey;
        }

        private void OnCaptchaCountChanged() {
            int cachedCount;
            int neededCount;
            int extraCount;
            lock (_settingsSync) {
                cachedCount = _captchas.Count;
                neededCount = Math.Max(_neededCaptchaCount, 0);
                extraCount = Math.Max(-_neededCaptchaCount, 0);
            }
            OnEvent(CaptchaCountChanged, new CaptchaCountChangedEventArgs(cachedCount, neededCount, extraCount));
        }

        private void OnEvent<TEventArgs>(EventHandler<ImageDumper, TEventArgs> evt, TEventArgs e)
            where TEventArgs : EventArgs 
        {
            lock (_eventSync) {
                lock (_settingsSync) {
                    if (!_isRunning && !(e is StoppedEventArgs)) return;
                }
                if (evt != null) {
                    try {
                        evt(this, e);
                    }
                    catch { }
                }
            }
        }

        public event EventHandler<ImageDumper, PostBeginEventArgs> PostBegin;

        public event EventHandler<ImageDumper, PostStatusEventArgs> PostStatus;

        public event EventHandler<ImageDumper, PostEndEventArgs> PostEnd;

        public event EventHandler<ImageDumper, EventArgs> CaptchaKeyFound;

        public event EventHandler<ImageDumper, CaptchaCountChangedEventArgs> CaptchaCountChanged;

        public event EventHandler<ImageDumper, CaptchaChallengeReceivedEventArgs> CaptchaChallengeReceived;

        public event EventHandler<ImageDumper, EventArgs> CaptchaChallengeReceiveError;

        public event EventHandler<ImageDumper, StoppedEventArgs> Stopped;
    }
}