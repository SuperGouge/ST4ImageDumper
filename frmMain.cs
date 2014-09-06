using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace JDP {
    public partial class frmMain : Form {
        private readonly ImageDumper _dumper = new ImageDumper();
        private PostState _postState;
        private bool _isCommentStale;

        public frmMain() {
            InitializeComponent();
            GUI.SetFontAndScaling(this);

            SetControlState(false);
            BindIntervalList();
            LoadSettings();

            _dumper.LoadCaptchasFromFile(Settings.CaptchasPath);
            _dumper.PostBegin += ImageDumper_PostBegin;
            _dumper.PostStatus += ImageDumper_PostStatus;
            _dumper.PostEnd += ImageDumper_PostEnd;
            _dumper.CaptchaKeyFound += ImageDumper_CaptchaKeyFound;
            _dumper.CaptchaCountChanged += ImageDumper_CaptchaCountChanged;
            _dumper.CaptchaChallengeReceived += ImageDumper_CaptchaChallengeReceived;
            _dumper.CaptchaChallengeReceiveError += ImageDumper_CaptchaChallengeReceiveError;
            _dumper.Stopped += ImageDumper_Stopped;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
            SaveSettings();

            _dumper.SaveCaptchasToFile(Settings.CaptchasPath);
        }

        private void ImageDumper_PostBegin(ImageDumper sender, PostBeginEventArgs e) {
            Invoke(() => {
                lblFileNumber.Text = (e.FileIndex + 1) + "/" + e.FileCount;
                llFileName.Text = GUI.LabelEscape(Path.GetFileName(e.FilePath));
                llFileName.Tag = e.FilePath;
            });
        }

        private void ImageDumper_PostStatus(ImageDumper sender, PostStatusEventArgs e) {
            Invoke(() => {
                _postState = e.State;
                tmrWaitCountdown.Enabled = e.State == PostState.FloodWait;
                DisplayStatus();
                if (e.State == PostState.Posting) {
                    SetCommentEditLock(true);
                }
            });
        }

        private void ImageDumper_PostEnd(ImageDumper sender, PostEndEventArgs e) {
            Invoke(() => {
                LogResult(e.Result);
                SetCommentEditLock(false);
                if (e.Result == PostResult.Success) {
                    ClearComment();
                }
            });
            Action<string, string> writeDebugFile = (nameFormat, contents) => {
                string path = Path.Combine(_dumper.ImageDirectory, String.Format(nameFormat, DateTime.UtcNow.Ticks));
                try {
                    File.WriteAllText(path, contents);
                }
                catch { }
            };
            if (e.Result == PostResult.UnrecognizedResponse) {
                writeDebugFile("_Dumper_Response_{0}.html", e.ResponseHTML);
            }
            else if (e.Result == PostResult.UnhandledException) {
                writeDebugFile("_Dumper_Exception_{0}.txt", e.UnhandledException.ToString());
            }
        }

        private void ImageDumper_CaptchaKeyFound(ImageDumper sender, EventArgs e) {
            Invoke(() => {
                grpCaptcha.Enabled = true;
            });
            _dumper.RequestCaptchaChallenge();
        }

        private void ImageDumper_CaptchaCountChanged(ImageDumper sender, CaptchaCountChangedEventArgs e) {
            Invoke(() => {
                grpCaptcha.Text = String.Format("Captcha ({0} cached, {1} needed, {2} extra)",
                    e.CachedCount, e.NeededCount, e.ExtraCount);
                if (picCaptcha.Tag != null) {
                    Captcha captcha = (Captcha)picCaptcha.Tag;
                    bool willExpire = WillCaptchaExpireBeforeUse(captcha);
                    lblCaptchaExpirationWarning.Visible = willExpire;
                    if (willExpire && (DateTime.UtcNow - captcha.TimeAcquiredUTC) >= TimeSpan.FromMinutes(1)) {
                        ClearCaptcha();
                        _dumper.RequestCaptchaChallenge();
                    }
                }
            });
        }

        private void ImageDumper_CaptchaChallengeReceived(ImageDumper sender, CaptchaChallengeReceivedEventArgs e) {
            Invoke(() => {
                ClearCaptcha();
                picCaptcha.Tag = e.Captcha;
                picCaptcha.Image = e.Image;
                txtCaptcha.Enabled = true;
                btnSubmitCaptcha.Enabled = true;
                btnRefreshCaptcha.Enabled = true;
                txtCaptcha.Focus();
                lblCaptchaExpirationWarning.Visible = WillCaptchaExpireBeforeUse(e.Captcha);
            });
        }

        private void ImageDumper_CaptchaChallengeReceiveError(ImageDumper sender, EventArgs e) {
            Invoke(() => {
                btnRefreshCaptcha.Enabled = true;
            });
        }

        private void ImageDumper_Stopped(ImageDumper sender, StoppedEventArgs e) {
            Invoke(() => {
                if (e.TriggeringException != null) {
                    MessageBox.Show(this, e.TriggeringException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                SetControlState(false);
                SetCommentEditLock(false);
                if (chkNumberPosts.Checked) {
                    txtPostNumberStart.Text = _dumper.PostNumberStart.ToString();
                }
            });
            _isCommentStale = true;
        }

        private void tmrWaitCountdown_Tick(object sender, EventArgs e) {
            if (!_dumper.IsRunning) return;
            DisplayStatus();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog browse = new FolderBrowserDialog()) {
                browse.ShowNewFolderButton = false;
                if (browse.ShowDialog(this) == DialogResult.OK) {
                    txtFolder.Text = browse.SelectedPath;
                }
            }
        }

        private void btnShowPassword_Click(object sender, EventArgs e) {
            bool show = btnShowPassword.Text == "Show";
            btnShowPassword.Text = show ? "Hide" : "Show";
            txtPassword.UseSystemPasswordChar = !show;
        }

        private void btnShowChanPassPIN_Click(object sender, EventArgs e) {
            bool show = btnShowChanPassPIN.Text == "Show";
            btnShowChanPassPIN.Text = show ? "Hide" : "Show";
            txtChanPassPIN.UseSystemPasswordChar = !show;
        }

        private void btnStart_Click(object sender, EventArgs e) {
            string folder = txtFolder.Text.Trim();
            string url = txtURL.Text.Trim();
            int postNumberStart;

            if (txtPassword.Text.Length == 0) {
                txtPassword.Text = General.Generate4chanPassword();
            }

            try {
                if (folder.Length == 0) {
                    throw new Exception("Folder is required.");
                }
                if (url.Length == 0) {
                    throw new Exception("URL is required.");
                }
                if (!Int32.TryParse(txtPostNumberStart.Text.Trim(), out postNumberStart) || postNumberStart < 1) {
                    throw new Exception("Invalid start at post number.");
                }
                url = General.CleanPageURL(url);
                if (url == null || url.IndexOf("/res/", StringComparison.OrdinalIgnoreCase) == -1) {
                    throw new Exception("Invalid thread URL.");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsEditingComment && txtComment.TextLength != 0 && _isCommentStale) {
                tabMain.SelectedTab = tpComment;
                if (MessageBox.Show(this, "The comment was entered during a previous posting session.  Do you want to post it anyway?",
                    "Post Comment?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    _dumper.NextPostComment = null;
                    ClearComment();
                }
            }

            txtURL.Text = url;
            SetControlState(true);
            tabMain.SelectedTab = tpPost;
            lblStatus.Text = "Initializing";
            txtResultLog.Clear();

            _dumper.ImageDirectory = folder;
            _dumper.ThreadURL = url;
            _dumper.PostInterval = TimeSpan.FromSeconds((int)cboInterval.SelectedValue);
            _dumper.NumberPosts = chkNumberPosts.Checked;
            _dumper.PostNumberStart = postNumberStart;
            _dumper.RandomizeOrder = chkRandomizeOrder.Checked;
            _dumper.MovePostedToSubdirectory = chkMovePostedToSubfolder.Checked;
            _dumper.UserName = txtName.Text.Trim();
            _dumper.EmailAddress = txtEmail.Text.Trim();
            _dumper.Password = txtPassword.Text;
            _dumper.ChanPassToken = txtChanPassToken.Text.Trim();
            _dumper.ChanPassPIN = txtChanPassPIN.Text.Trim();

            _dumper.Start();
        }

        private void btnPause_Click(object sender, EventArgs e) {
            bool isPaused = !_dumper.IsPaused;
            _dumper.IsPaused = isPaused;
            btnPause.Text = isPaused ? "Resume" : "Pause";
        }

        private void btnStop_Click(object sender, EventArgs e) {
            if (!_dumper.IsStopping) {
                _dumper.Stop();
            }
            else {
                DialogResult dialogResult = MessageBox.Show(this, "Would you like to force stop without finishing cleanly?",
                    "Force Stop", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes) {
                    _dumper.Abort();
                }
            }
        }

        private void btnAbout_Click(object sender, EventArgs e) {
            MessageBox.Show(this, String.Format("ST4 Image Dumper{0}Version {1} ({2}){0}Author: JDP (jart1126@yahoo.com){0}{3}",
                Environment.NewLine, General.Version, General.ReleaseDate, General.ApplicationURL), "About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void lblFolder_DoubleClick(object sender, EventArgs e) {
            string folder = txtFolder.Text.Trim();
            if (folder.Length == 0 || !Directory.Exists(folder)) return;
            ThreadPool.QueueUserWorkItem((s) => {
                Process.Start(folder);
            });
        }

        private void lblURL_DoubleClick(object sender, EventArgs e) {
            string url = General.CleanPageURL(txtURL.Text.Trim());
            if (url == null) return;
            ThreadPool.QueueUserWorkItem((s) => {
                Process.Start(url);
            });
        }

        private void txtURL_DragEnter(object sender, DragEventArgs e) {
            GUI.URLDropOnDragEnter(e);
        }

        private void txtURL_DragDrop(object sender, DragEventArgs e) {
            string url = GUI.URLDropOnDragDrop(e);
            if (url != null) {
                txtURL.Text = url;
            }
        }

        private void txtFolder_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void txtFolder_DragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths.Length != 1 || !Directory.Exists(paths[0])) return;
                txtFolder.Text = paths[0];
            }
        }

        private void chkNumberPosts_CheckedChanged(object sender, EventArgs e) {
            txtPostNumberStart.Enabled = chkNumberPosts.Checked;
        }

        private void lblCaptchaExpirationWarning_DoubleClick(object sender, EventArgs e) {
            lblCaptchaExpirationWarning.Visible = false;
        }

        private void txtCaptcha_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                btnSubmitCaptcha_Click(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F5) {
                btnRefreshCaptcha_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void btnSubmitCaptcha_Click(object sender, EventArgs e) {
            if (txtCaptcha.Text.Length == 0) return;

            Captcha captcha = (Captcha)picCaptcha.Tag;
            captcha.Response = txtCaptcha.Text;

            ClearCaptcha();
            _dumper.AddCaptcha(captcha);
            _dumper.RequestCaptchaChallenge();
        }

        private void btnRefreshCaptcha_Click(object sender, EventArgs e) {
            ClearCaptcha();
            _dumper.RequestCaptchaChallenge();
        }

        private void llFileName_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            string path = (string)llFileName.Tag;
            if (path == null || !File.Exists(path)) return;
            ThreadPool.QueueUserWorkItem((s) => {
                Process.Start(path);
            });
        }

        private void btnEditComment_Click(object sender, EventArgs e) {
            btnEditComment.Enabled = false;
            txtComment.Enabled = true;
            btnEditCommentDone.Visible = true;
            lblIncludeComment.Text = "No (editing...)";
            _dumper.NextPostComment = null;
            txtComment.Focus();
        }

        private void btnEditCommentDone_Click(object sender, EventArgs e) {
            btnEditComment.Enabled = true;
            txtComment.Enabled = false;
            btnEditCommentDone.Visible = false;
            _isCommentStale = false;
            if (txtComment.Text.Trim().Length != 0) {
                lblIncludeComment.Text = "Yes";
                _dumper.NextPostComment = txtComment.Text;
            }
            else {
                ClearComment();
            }
            if (_dumper.IsRunning) {
                tabMain.SelectedTab = tpPost;
            }
        }

        private void DisplayStatus() {
            string text = GetStatusDescription();
            if (lblStatus.Text != text) {
                lblStatus.Text = text;
            }
        }

        private string GetStatusDescription() {
            switch (_postState) {
                case PostState.FloodWait:
                    return String.Format("Waiting {0} seconds", Math.Ceiling(_dumper.FloodWaitRemainingDuration.TotalSeconds));
                case PostState.CaptchaWait:
                    return "Waiting for captcha input";
                case PostState.Paused:
                    return "Paused";
                case PostState.Posting:
                    return "Posting image";
                default:
                    return "Unknown";
            }
        }

        private void LogResult(PostResult result) {
            string text = String.Format("[{0:HH:mm:ss}] {1} ({2}: {3})",
                DateTime.Now,
                GetResultDescription(result),
                lblFileNumber.Text,
                GUI.LabelUnescape(llFileName.Text));
            txtResultLog.SuspendLayout();
            txtResultLog.AppendText(txtResultLog.Text.Length != 0 ? (Environment.NewLine + text) : text);
            txtResultLog.SelectionStart = txtResultLog.Text.Length - text.Length;
            txtResultLog.ScrollToCaret();
            txtResultLog.ResumeLayout();
        }

        private string GetResultDescription(PostResult result) {
            switch (result) {
                case PostResult.Success:
                    return "Posted successfully";
                case PostResult.FileNotFound:
                    return "File not found";
                case PostResult.IncorrectCaptcha:
                    return "Incorrect captcha";
                case PostResult.ChanPassNotAccepted:
                    return "4chan Pass not accepted";
                case PostResult.FloodDetected:
                    return "Flood detected";
                case PostResult.DuplicateFile:
                    return "Duplicate file";
                case PostResult.FileNotAllowed:
                    return "File is not allowed";
                case PostResult.FileTooLarge:
                    return "File is too large";
                case PostResult.CorruptFile:
                    return "Corrupt file";
                case PostResult.ContainsEmbeddedFile:
                    return "Image contains an embedded file";
                case PostResult.MaliciousFile:
                    return "Possible malicious file";
                case PostResult.FieldTooLong:
                    return "Form field is too long";
                case PostResult.ThreadFull:
                    return "Thread is full";
                case PostResult.ThreadDead:
                    return "Thread no longer exists";
                case PostResult.UnrecognizedResponse:
                    return "Unrecognized response";
                case PostResult.UnhandledException:
                    return "Unhandled exception";
                default:
                    return "Unknown";
            }
        }

        private void SetControlState(bool isRunning) {
            TopMost = isRunning;
            txtFolder.Enabled = !isRunning;
            btnBrowseFolder.Enabled = !isRunning;
            txtURL.Enabled = !isRunning;
            cboInterval.Enabled = !isRunning;
            chkNumberPosts.Enabled = !isRunning;
            txtPostNumberStart.Enabled = !isRunning && chkNumberPosts.Checked;
            chkRandomizeOrder.Enabled = !isRunning;
            chkMovePostedToSubfolder.Enabled = !isRunning;
            txtName.Enabled = !isRunning;
            txtEmail.Enabled = !isRunning;
            txtPassword.Enabled = !isRunning;
            txtChanPassToken.Enabled = !isRunning;
            txtChanPassPIN.Enabled = !isRunning;

            btnStart.Enabled = !isRunning;
            btnPause.Enabled = isRunning;
            btnStop.Enabled = isRunning;

            if (!isRunning) {
                tmrWaitCountdown.Enabled = false;
                btnPause.Text = "Pause";
                grpCaptcha.Enabled = false;
                grpCaptcha.Text = "Captcha";
                ClearCaptcha();
                lblFileNumber.Text = "N/A";
                llFileName.Text = String.Empty;
                llFileName.Tag = null;
                lblStatus.Text = "Stopped";
            }
        }

        private void SetCommentEditLock(bool locked) {
            if (IsEditingComment) {
                btnEditCommentDone.Enabled = !locked;
            }
            else {
                btnEditComment.Enabled = !locked;
            }
        }

        private void ClearComment() {
            if (!IsEditingComment) {
                lblIncludeComment.Text = "No";
                txtComment.Clear();
            }
        }

        private bool IsEditingComment {
            get { return txtComment.Enabled; }
        }

        private bool WillCaptchaExpireBeforeUse(Captcha captcha) {
            int cachedCount = 0;
            TimeSpan floodWaitRemainingDuration = TimeSpan.Zero;
            bool currentPostAcquiredCaptcha = false;
            _dumper.AtomicSettingsAction(() => {
                cachedCount = _dumper.CachedCaptchaCount;
                floodWaitRemainingDuration = _dumper.FloodWaitRemainingDuration;
                currentPostAcquiredCaptcha = _dumper.CurrentPostAcquiredCaptcha;
            });
            TimeSpan captchaUseInterval = _dumper.PostInterval + _dumper.AveragePostDuration;
            TimeSpan firstCaptchaUseDelay = !currentPostAcquiredCaptcha ?
                (floodWaitRemainingDuration + _dumper.AveragePostDuration) :
                (_dumper.AveragePostDuration + captchaUseInterval);
            DateTime estimatedUseTimeUTC = DateTime.UtcNow + firstCaptchaUseDelay +
                                           TimeSpan.FromTicks(cachedCount * captchaUseInterval.Ticks);
            return estimatedUseTimeUTC > (captcha.ExpirationTimeUTC - _dumper.CaptchaExpirationThreshold);
        }

        private void ClearCaptcha() {
            GUI.ClearImage(picCaptcha);
            picCaptcha.Tag = null;
            txtCaptcha.Clear();
            txtCaptcha.Enabled = false;
            lblCaptchaExpirationWarning.Visible = false;
            btnSubmitCaptcha.Enabled = false;
            btnRefreshCaptcha.Enabled = false;
        }

        private void BindIntervalList() {
            cboInterval.ValueMember = "Value";
            cboInterval.DisplayMember = "Text";
            cboInterval.DataSource = new[] {
                new ListItemInt32(30, "30 seconds"),
                new ListItemInt32(45, "45 seconds"),
                new ListItemInt32(60, "1 minute"),
                new ListItemInt32(120, "2 minutes"),
            };
        }

        private void LoadSettings() {
            txtFolder.Text = Settings.ImageFolder;
            txtURL.Text = Settings.ThreadURL;
            GUI.SelectListValueOrDefault(cboInterval, Settings.PostInterval, 45);
            chkNumberPosts.Checked = Settings.NumberPosts ?? false;
            txtPostNumberStart.Text = (Settings.PostNumberStart ?? 1).ToString();
            chkRandomizeOrder.Checked = Settings.RandomizeOrder ?? false;
            chkMovePostedToSubfolder.Checked = Settings.MovePostedToSubfolder ?? false;
            txtName.Text = Settings.UserName;
            txtEmail.Text = Settings.EmailAddress;
            txtPassword.Text = Settings.Password;
            txtChanPassToken.Text = Settings.ChanPassToken;
            txtChanPassPIN.Text = Settings.ChanPassPIN;
        }

        private void SaveSettings() {
            Settings.ImageFolder = txtFolder.Text.Trim();
            Settings.ThreadURL = txtURL.Text.Trim();
            Settings.PostInterval = (int)cboInterval.SelectedValue;
            Settings.NumberPosts = chkNumberPosts.Checked;
            Settings.PostNumberStart = General.TryParseInt32(txtPostNumberStart.Text.Trim());
            Settings.RandomizeOrder = chkRandomizeOrder.Checked;
            Settings.MovePostedToSubfolder = chkMovePostedToSubfolder.Checked;
            Settings.UserName = txtName.Text.Trim();
            Settings.EmailAddress = txtEmail.Text.Trim();
            Settings.Password = txtPassword.Text;
            Settings.ChanPassToken = txtChanPassToken.Text.Trim();
            Settings.ChanPassPIN = txtChanPassPIN.Text.Trim();
            Settings.Save();
        }

        private object Invoke(MethodInvoker method) {
            return Invoke((Delegate)method);
        }
    }
}