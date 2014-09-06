using System;
using System.Collections.Generic;
using System.Drawing;

namespace JDP {
    public static class ExtensionMethods {
        public static TextFormField GetTextField(this List<FormField> list, string name) {
            TextFormField field = (TextFormField)list.Find(f =>
                f is TextFormField && f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (field == null) {
                throw new Exception("Unable to find a text field named \"" + name + "\".");
            }
            return field;
        }

        public static FileFormField GetFileField(this List<FormField> list, string name) {
            FileFormField field = (FileFormField)list.Find(f =>
                f is FileFormField && f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (field == null) {
                throw new Exception("Unable to find a file field named \"" + name + "\".");
            }
            return field;
        }
    }

    public static class TickCount {
        private static object _sync = new object();
        private static int _lastTickCount;
        private static long _correction;

        public static long Now {
            get {
                lock (_sync) {
                    int tickCount = Environment.TickCount;
                    if ((tickCount < 0) && (_lastTickCount >= 0)) {
                        _correction += 0x100000000L;
                    }
                    _lastTickCount = tickCount;
                    return tickCount + _correction;
                }
            }
        }
    }

    public class Captcha {
        public string PublicKey { get; set; }
        public DateTime TimeAcquiredUTC { get; set; }
        public int TimeoutSeconds { get; set; }
        public string Challenge { get; set; }
        public string Response { get; set; }

        public DateTime ExpirationTimeUTC {
            get {
                return TimeAcquiredUTC + TimeSpan.FromSeconds(TimeoutSeconds);
            }
        }

        public bool IsExpired(TimeSpan threshold) {
            return DateTime.UtcNow > ExpirationTimeUTC - threshold;
        }
    }

    public class CustomComparer<T> : IComparer<T> {
        private Comparison<T> _comparison;

        public CustomComparer(Comparison<T> comparison) {
            _comparison = comparison;
        }

        public int Compare(T x, T y) {
            return _comparison(x, y);
        }
    }

    public class ResponseClassifier {
        public string IndicatorText { get; private set; }
        public PostResult ResponseResult { get; private set; }

        public ResponseClassifier(string indicatorText, PostResult responseResult) {
            IndicatorText = indicatorText;
            ResponseResult = responseResult;
        }

        public bool IsMatch(string responseHTML) {
            return responseHTML.IndexOf(IndicatorText, StringComparison.OrdinalIgnoreCase) != -1;
        }
    }

    public class StopException : Exception {
    }

    public class FatalPostException : Exception {
        public FatalPostException(string message) :
            base(message) { }
    }

    public class PostBeginEventArgs : EventArgs {
        public int FileIndex { get; private set; }
        public int FileCount { get; private set; }
        public string FilePath { get; private set; }

        public PostBeginEventArgs(int fileIndex, int fileCount, string filePath) {
            FileIndex = fileIndex;
            FileCount = fileCount;
            FilePath = filePath;
        }
    }

    public class PostStatusEventArgs : EventArgs {
        public PostState State { get; private set; }

        public PostStatusEventArgs(PostState state) {
            State = state;
        }
    }

    public class PostEndEventArgs : EventArgs {
        public PostResult Result { get; private set; }
        public string ResponseHTML { get; private set; }
        public Exception UnhandledException { get; private set; }

        public PostEndEventArgs(PostResult result, string responseHTML, Exception unhandledException) {
            Result = result;
            ResponseHTML = responseHTML;
            UnhandledException = unhandledException;
        }

        public PostEndEventArgs(PostResult result) :
            this(result, null, null) { }
    }

    public class CaptchaCountChangedEventArgs : EventArgs {
        public int CachedCount { get; private set; }
        public int NeededCount { get; private set; }
        public int ExtraCount { get; private set; }

        public CaptchaCountChangedEventArgs(int cachedCount, int neededCount, int extraCount) {
            CachedCount = cachedCount;
            NeededCount = neededCount;
            ExtraCount = extraCount;
        }
    }

    public class CaptchaChallengeReceivedEventArgs : EventArgs {
        public Image Image { get; private set; }
        public Captcha Captcha { get; private set; }

        public CaptchaChallengeReceivedEventArgs(Image image, Captcha captcha) {
            Image = image;
            Captcha = captcha;
        }
    }

    public class StoppedEventArgs : EventArgs {
        public Exception TriggeringException { get; private set; }

        public StoppedEventArgs(Exception triggeringException) {
            TriggeringException = triggeringException;
        }
    }

    public enum PostState {
        FloodWait,
        CaptchaWait,
        Paused,
        Posting
    }

    public enum PostResult {
        Success,
        FileNotFound,
        IncorrectCaptcha,
        ChanPassNotAccepted,
        FloodDetected,
        DuplicateFile,
        FileNotAllowed,
        FileTooLarge,
        CorruptFile,
        ContainsEmbeddedFile,
        MaliciousFile,
        FieldTooLong,
        ThreadFull,
        ThreadDead,
        Banned,
        Blocked,
        Copyrighted,
        UnrecognizedResponse,
        UnhandledException
    }

    public delegate void EventHandler<TSender, TEventArgs>(TSender sender, TEventArgs e) where TEventArgs : EventArgs;
}