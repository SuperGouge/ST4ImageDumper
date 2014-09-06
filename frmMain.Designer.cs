namespace JDP {
    partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSubmitCaptcha = new System.Windows.Forms.Button();
            this.txtCaptcha = new System.Windows.Forms.TextBox();
            this.btnRefreshCaptcha = new System.Windows.Forms.Button();
            this.picCaptcha = new System.Windows.Forms.PictureBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.lblFolder = new System.Windows.Forms.Label();
            this.grpCaptcha = new System.Windows.Forms.GroupBox();
            this.lblCaptchaExpirationWarning = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.txtResultLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatusLabel = new System.Windows.Forms.Label();
            this.llFileName = new System.Windows.Forms.LinkLabel();
            this.lblFileNumber = new System.Windows.Forms.Label();
            this.lblFileLabel = new System.Windows.Forms.Label();
            this.txtPostNumberStart = new System.Windows.Forms.TextBox();
            this.chkNumberPosts = new System.Windows.Forms.CheckBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.tmrWaitCountdown = new System.Windows.Forms.Timer(this.components);
            this.cboInterval = new System.Windows.Forms.ComboBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.btnShowPassword = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.chkMovePostedToSubfolder = new System.Windows.Forms.CheckBox();
            this.chkRandomizeOrder = new System.Windows.Forms.CheckBox();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.tpPost = new System.Windows.Forms.TabPage();
            this.tpComment = new System.Windows.Forms.TabPage();
            this.btnEditCommentDone = new System.Windows.Forms.Button();
            this.btnEditComment = new System.Windows.Forms.Button();
            this.lblIncludeComment = new System.Windows.Forms.Label();
            this.lblIncludeCommentLabel = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.grpPosting = new System.Windows.Forms.GroupBox();
            this.grpUser = new System.Windows.Forms.GroupBox();
            this.grpChanPass = new System.Windows.Forms.GroupBox();
            this.txtChanPassToken = new System.Windows.Forms.TextBox();
            this.lblChanPassToken = new System.Windows.Forms.Label();
            this.lblChanPassPIN = new System.Windows.Forms.Label();
            this.txtChanPassPIN = new System.Windows.Forms.TextBox();
            this.btnShowChanPassPIN = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picCaptcha)).BeginInit();
            this.grpCaptcha.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tpSettings.SuspendLayout();
            this.tpPost.SuspendLayout();
            this.tpComment.SuspendLayout();
            this.grpPosting.SuspendLayout();
            this.grpUser.SuspendLayout();
            this.grpChanPass.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(71, 424);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(56, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSubmitCaptcha
            // 
            this.btnSubmitCaptcha.Location = new System.Drawing.Point(176, 84);
            this.btnSubmitCaptcha.Name = "btnSubmitCaptcha";
            this.btnSubmitCaptcha.Size = new System.Drawing.Size(64, 23);
            this.btnSubmitCaptcha.TabIndex = 2;
            this.btnSubmitCaptcha.Text = "Submit";
            this.btnSubmitCaptcha.UseVisualStyleBackColor = true;
            this.btnSubmitCaptcha.Click += new System.EventHandler(this.btnSubmitCaptcha_Click);
            // 
            // txtCaptcha
            // 
            this.txtCaptcha.Location = new System.Drawing.Point(12, 84);
            this.txtCaptcha.Name = "txtCaptcha";
            this.txtCaptcha.Size = new System.Drawing.Size(156, 20);
            this.txtCaptcha.TabIndex = 1;
            this.txtCaptcha.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCaptcha_KeyDown);
            // 
            // btnRefreshCaptcha
            // 
            this.btnRefreshCaptcha.Location = new System.Drawing.Point(248, 84);
            this.btnRefreshCaptcha.Name = "btnRefreshCaptcha";
            this.btnRefreshCaptcha.Size = new System.Drawing.Size(64, 23);
            this.btnRefreshCaptcha.TabIndex = 3;
            this.btnRefreshCaptcha.Text = "Refresh";
            this.btnRefreshCaptcha.UseVisualStyleBackColor = true;
            this.btnRefreshCaptcha.Click += new System.EventHandler(this.btnRefreshCaptcha_Click);
            // 
            // picCaptcha
            // 
            this.picCaptcha.Location = new System.Drawing.Point(12, 20);
            this.picCaptcha.Name = "picCaptcha";
            this.picCaptcha.Size = new System.Drawing.Size(300, 58);
            this.picCaptcha.TabIndex = 4;
            this.picCaptcha.TabStop = false;
            // 
            // txtFolder
            // 
            this.txtFolder.AllowDrop = true;
            this.txtFolder.Location = new System.Drawing.Point(64, 16);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(268, 20);
            this.txtFolder.TabIndex = 1;
            this.txtFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtFolder_DragDrop);
            this.txtFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtFolder_DragEnter);
            // 
            // txtURL
            // 
            this.txtURL.AllowDrop = true;
            this.txtURL.Location = new System.Drawing.Point(64, 44);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(268, 20);
            this.txtURL.TabIndex = 4;
            this.txtURL.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtURL_DragDrop);
            this.txtURL.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtURL_DragEnter);
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(8, 48);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(32, 13);
            this.lblURL.TabIndex = 3;
            this.lblURL.Text = "URL:";
            this.lblURL.DoubleClick += new System.EventHandler(this.lblURL_DoubleClick);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(8, 20);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(39, 13);
            this.lblFolder.TabIndex = 0;
            this.lblFolder.Text = "Folder:";
            this.lblFolder.DoubleClick += new System.EventHandler(this.lblFolder_DoubleClick);
            // 
            // grpCaptcha
            // 
            this.grpCaptcha.Controls.Add(this.lblCaptchaExpirationWarning);
            this.grpCaptcha.Controls.Add(this.picCaptcha);
            this.grpCaptcha.Controls.Add(this.btnSubmitCaptcha);
            this.grpCaptcha.Controls.Add(this.txtCaptcha);
            this.grpCaptcha.Controls.Add(this.btnRefreshCaptcha);
            this.grpCaptcha.Location = new System.Drawing.Point(38, 4);
            this.grpCaptcha.Name = "grpCaptcha";
            this.grpCaptcha.Size = new System.Drawing.Size(324, 116);
            this.grpCaptcha.TabIndex = 0;
            this.grpCaptcha.TabStop = false;
            this.grpCaptcha.Text = "Captcha";
            // 
            // lblCaptchaExpirationWarning
            // 
            this.lblCaptchaExpirationWarning.BackColor = System.Drawing.Color.Red;
            this.lblCaptchaExpirationWarning.Location = new System.Drawing.Point(12, 20);
            this.lblCaptchaExpirationWarning.Name = "lblCaptchaExpirationWarning";
            this.lblCaptchaExpirationWarning.Size = new System.Drawing.Size(300, 16);
            this.lblCaptchaExpirationWarning.TabIndex = 0;
            this.lblCaptchaExpirationWarning.Text = "Captcha may expire before use.";
            this.lblCaptchaExpirationWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCaptchaExpirationWarning.DoubleClick += new System.EventHandler(this.lblCaptchaExpirationWarning_DoubleClick);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(211, 424);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(56, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.txtResultLog);
            this.grpStatus.Controls.Add(this.lblStatus);
            this.grpStatus.Controls.Add(this.lblStatusLabel);
            this.grpStatus.Controls.Add(this.llFileName);
            this.grpStatus.Controls.Add(this.lblFileNumber);
            this.grpStatus.Controls.Add(this.lblFileLabel);
            this.grpStatus.Location = new System.Drawing.Point(8, 124);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(384, 248);
            this.grpStatus.TabIndex = 1;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // txtResultLog
            // 
            this.txtResultLog.Location = new System.Drawing.Point(12, 64);
            this.txtResultLog.Multiline = true;
            this.txtResultLog.Name = "txtResultLog";
            this.txtResultLog.ReadOnly = true;
            this.txtResultLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResultLog.Size = new System.Drawing.Size(360, 172);
            this.txtResultLog.TabIndex = 5;
            this.txtResultLog.WordWrap = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Location = new System.Drawing.Point(60, 40);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(312, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Posting image";
            // 
            // lblStatusLabel
            // 
            this.lblStatusLabel.Location = new System.Drawing.Point(12, 40);
            this.lblStatusLabel.Name = "lblStatusLabel";
            this.lblStatusLabel.Size = new System.Drawing.Size(42, 13);
            this.lblStatusLabel.TabIndex = 3;
            this.lblStatusLabel.Text = "Status:";
            // 
            // llFileName
            // 
            this.llFileName.AutoEllipsis = true;
            this.llFileName.Location = new System.Drawing.Point(104, 20);
            this.llFileName.Name = "llFileName";
            this.llFileName.Size = new System.Drawing.Size(268, 13);
            this.llFileName.TabIndex = 2;
            this.llFileName.TabStop = true;
            this.llFileName.Text = "Test.jpg";
            this.llFileName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llFileName_LinkClicked);
            // 
            // lblFileNumber
            // 
            this.lblFileNumber.Location = new System.Drawing.Point(48, 20);
            this.lblFileNumber.Name = "lblFileNumber";
            this.lblFileNumber.Size = new System.Drawing.Size(50, 13);
            this.lblFileNumber.TabIndex = 1;
            this.lblFileNumber.Text = "100/100";
            // 
            // lblFileLabel
            // 
            this.lblFileLabel.Location = new System.Drawing.Point(12, 20);
            this.lblFileLabel.Name = "lblFileLabel";
            this.lblFileLabel.Size = new System.Drawing.Size(28, 13);
            this.lblFileLabel.TabIndex = 0;
            this.lblFileLabel.Text = "File:";
            // 
            // txtPostNumberStart
            // 
            this.txtPostNumberStart.Enabled = false;
            this.txtPostNumberStart.Location = new System.Drawing.Point(224, 100);
            this.txtPostNumberStart.Name = "txtPostNumberStart";
            this.txtPostNumberStart.Size = new System.Drawing.Size(40, 20);
            this.txtPostNumberStart.TabIndex = 8;
            this.txtPostNumberStart.Text = "1";
            // 
            // chkNumberPosts
            // 
            this.chkNumberPosts.AutoSize = true;
            this.chkNumberPosts.Location = new System.Drawing.Point(10, 102);
            this.chkNumberPosts.Name = "chkNumberPosts";
            this.chkNumberPosts.Size = new System.Drawing.Size(202, 17);
            this.chkNumberPosts.TabIndex = 7;
            this.chkNumberPosts.Text = "Include progress in comment, start at:";
            this.chkNumberPosts.UseVisualStyleBackColor = true;
            this.chkNumberPosts.CheckedChanged += new System.EventHandler(this.chkNumberPosts_CheckedChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(72, 16);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 20);
            this.txtName.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(8, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(275, 424);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(60, 23);
            this.btnAbout.TabIndex = 4;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(135, 424);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(68, 23);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // tmrWaitCountdown
            // 
            this.tmrWaitCountdown.Interval = 250;
            this.tmrWaitCountdown.Tick += new System.EventHandler(this.tmrWaitCountdown_Tick);
            // 
            // cboInterval
            // 
            this.cboInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInterval.FormattingEnabled = true;
            this.cboInterval.Location = new System.Drawing.Point(64, 72);
            this.cboInterval.Name = "cboInterval";
            this.cboInterval.Size = new System.Drawing.Size(100, 21);
            this.cboInterval.TabIndex = 6;
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(8, 76);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(45, 13);
            this.lblInterval.TabIndex = 5;
            this.lblInterval.Text = "Interval:";
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tpSettings);
            this.tabMain.Controls.Add(this.tpPost);
            this.tabMain.Controls.Add(this.tpComment);
            this.tabMain.Location = new System.Drawing.Point(8, 8);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(408, 407);
            this.tabMain.TabIndex = 0;
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.grpChanPass);
            this.tpSettings.Controls.Add(this.grpUser);
            this.tpSettings.Controls.Add(this.grpPosting);
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tpSettings.Size = new System.Drawing.Size(400, 381);
            this.tpSettings.TabIndex = 0;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // btnShowPassword
            // 
            this.btnShowPassword.Location = new System.Drawing.Point(200, 72);
            this.btnShowPassword.Name = "btnShowPassword";
            this.btnShowPassword.Size = new System.Drawing.Size(56, 23);
            this.btnShowPassword.TabIndex = 6;
            this.btnShowPassword.Text = "Show";
            this.btnShowPassword.UseVisualStyleBackColor = true;
            this.btnShowPassword.Click += new System.EventHandler(this.btnShowPassword_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(72, 72);
            this.txtPassword.MaxLength = 8;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(120, 20);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(8, 76);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(72, 44);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(220, 20);
            this.txtEmail.TabIndex = 3;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(8, 48);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(35, 13);
            this.lblEmail.TabIndex = 2;
            this.lblEmail.Text = "Email:";
            // 
            // chkMovePostedToSubfolder
            // 
            this.chkMovePostedToSubfolder.AutoSize = true;
            this.chkMovePostedToSubfolder.Location = new System.Drawing.Point(10, 154);
            this.chkMovePostedToSubfolder.Name = "chkMovePostedToSubfolder";
            this.chkMovePostedToSubfolder.Size = new System.Drawing.Size(182, 17);
            this.chkMovePostedToSubfolder.TabIndex = 10;
            this.chkMovePostedToSubfolder.Text = "Move posted images to subfolder";
            this.chkMovePostedToSubfolder.UseVisualStyleBackColor = true;
            // 
            // chkRandomizeOrder
            // 
            this.chkRandomizeOrder.AutoSize = true;
            this.chkRandomizeOrder.Location = new System.Drawing.Point(10, 128);
            this.chkRandomizeOrder.Name = "chkRandomizeOrder";
            this.chkRandomizeOrder.Size = new System.Drawing.Size(159, 17);
            this.chkRandomizeOrder.TabIndex = 9;
            this.chkRandomizeOrder.Text = "Post images in random order";
            this.chkRandomizeOrder.UseVisualStyleBackColor = true;
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(340, 16);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(32, 23);
            this.btnBrowseFolder.TabIndex = 2;
            this.btnBrowseFolder.Text = "...";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // tpPost
            // 
            this.tpPost.Controls.Add(this.grpStatus);
            this.tpPost.Controls.Add(this.grpCaptcha);
            this.tpPost.Location = new System.Drawing.Point(4, 22);
            this.tpPost.Name = "tpPost";
            this.tpPost.Padding = new System.Windows.Forms.Padding(3);
            this.tpPost.Size = new System.Drawing.Size(400, 381);
            this.tpPost.TabIndex = 1;
            this.tpPost.Text = "Post";
            this.tpPost.UseVisualStyleBackColor = true;
            // 
            // tpComment
            // 
            this.tpComment.Controls.Add(this.btnEditCommentDone);
            this.tpComment.Controls.Add(this.btnEditComment);
            this.tpComment.Controls.Add(this.lblIncludeComment);
            this.tpComment.Controls.Add(this.lblIncludeCommentLabel);
            this.tpComment.Controls.Add(this.txtComment);
            this.tpComment.Location = new System.Drawing.Point(4, 22);
            this.tpComment.Name = "tpComment";
            this.tpComment.Padding = new System.Windows.Forms.Padding(3);
            this.tpComment.Size = new System.Drawing.Size(400, 381);
            this.tpComment.TabIndex = 2;
            this.tpComment.Text = "Comment";
            this.tpComment.UseVisualStyleBackColor = true;
            // 
            // btnEditCommentDone
            // 
            this.btnEditCommentDone.Location = new System.Drawing.Point(336, 232);
            this.btnEditCommentDone.Name = "btnEditCommentDone";
            this.btnEditCommentDone.Size = new System.Drawing.Size(56, 23);
            this.btnEditCommentDone.TabIndex = 4;
            this.btnEditCommentDone.Text = "Done";
            this.btnEditCommentDone.UseVisualStyleBackColor = true;
            this.btnEditCommentDone.Visible = false;
            this.btnEditCommentDone.Click += new System.EventHandler(this.btnEditCommentDone_Click);
            // 
            // btnEditComment
            // 
            this.btnEditComment.Location = new System.Drawing.Point(340, 10);
            this.btnEditComment.Name = "btnEditComment";
            this.btnEditComment.Size = new System.Drawing.Size(52, 23);
            this.btnEditComment.TabIndex = 2;
            this.btnEditComment.Text = "Edit";
            this.btnEditComment.UseVisualStyleBackColor = true;
            this.btnEditComment.Click += new System.EventHandler(this.btnEditComment_Click);
            // 
            // lblIncludeComment
            // 
            this.lblIncludeComment.AutoSize = true;
            this.lblIncludeComment.Location = new System.Drawing.Point(164, 14);
            this.lblIncludeComment.Name = "lblIncludeComment";
            this.lblIncludeComment.Size = new System.Drawing.Size(21, 13);
            this.lblIncludeComment.TabIndex = 1;
            this.lblIncludeComment.Text = "No";
            // 
            // lblIncludeCommentLabel
            // 
            this.lblIncludeCommentLabel.AutoSize = true;
            this.lblIncludeCommentLabel.Location = new System.Drawing.Point(8, 14);
            this.lblIncludeCommentLabel.Name = "lblIncludeCommentLabel";
            this.lblIncludeCommentLabel.Size = new System.Drawing.Size(148, 13);
            this.lblIncludeCommentLabel.TabIndex = 0;
            this.lblIncludeCommentLabel.Text = "Include comment in next post:";
            // 
            // txtComment
            // 
            this.txtComment.Enabled = false;
            this.txtComment.Location = new System.Drawing.Point(8, 42);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComment.Size = new System.Drawing.Size(384, 180);
            this.txtComment.TabIndex = 3;
            // 
            // grpPosting
            // 
            this.grpPosting.Controls.Add(this.txtFolder);
            this.grpPosting.Controls.Add(this.lblFolder);
            this.grpPosting.Controls.Add(this.txtURL);
            this.grpPosting.Controls.Add(this.lblURL);
            this.grpPosting.Controls.Add(this.cboInterval);
            this.grpPosting.Controls.Add(this.lblInterval);
            this.grpPosting.Controls.Add(this.chkMovePostedToSubfolder);
            this.grpPosting.Controls.Add(this.chkRandomizeOrder);
            this.grpPosting.Controls.Add(this.btnBrowseFolder);
            this.grpPosting.Controls.Add(this.txtPostNumberStart);
            this.grpPosting.Controls.Add(this.chkNumberPosts);
            this.grpPosting.Location = new System.Drawing.Point(8, 4);
            this.grpPosting.Name = "grpPosting";
            this.grpPosting.Size = new System.Drawing.Size(384, 180);
            this.grpPosting.TabIndex = 0;
            this.grpPosting.TabStop = false;
            this.grpPosting.Text = "Posting";
            // 
            // grpUser
            // 
            this.grpUser.Controls.Add(this.txtName);
            this.grpUser.Controls.Add(this.lblName);
            this.grpUser.Controls.Add(this.btnShowPassword);
            this.grpUser.Controls.Add(this.lblEmail);
            this.grpUser.Controls.Add(this.txtPassword);
            this.grpUser.Controls.Add(this.txtEmail);
            this.grpUser.Controls.Add(this.lblPassword);
            this.grpUser.Location = new System.Drawing.Point(8, 188);
            this.grpUser.Name = "grpUser";
            this.grpUser.Size = new System.Drawing.Size(384, 104);
            this.grpUser.TabIndex = 1;
            this.grpUser.TabStop = false;
            this.grpUser.Text = "User";
            // 
            // grpChanPass
            // 
            this.grpChanPass.Controls.Add(this.btnShowChanPassPIN);
            this.grpChanPass.Controls.Add(this.txtChanPassToken);
            this.grpChanPass.Controls.Add(this.lblChanPassToken);
            this.grpChanPass.Controls.Add(this.lblChanPassPIN);
            this.grpChanPass.Controls.Add(this.txtChanPassPIN);
            this.grpChanPass.Location = new System.Drawing.Point(8, 296);
            this.grpChanPass.Name = "grpChanPass";
            this.grpChanPass.Size = new System.Drawing.Size(384, 76);
            this.grpChanPass.TabIndex = 2;
            this.grpChanPass.TabStop = false;
            this.grpChanPass.Text = "4chan Pass";
            // 
            // txtChanPassToken
            // 
            this.txtChanPassToken.Location = new System.Drawing.Point(60, 16);
            this.txtChanPassToken.Name = "txtChanPassToken";
            this.txtChanPassToken.Size = new System.Drawing.Size(112, 20);
            this.txtChanPassToken.TabIndex = 1;
            // 
            // lblChanPassToken
            // 
            this.lblChanPassToken.AutoSize = true;
            this.lblChanPassToken.Location = new System.Drawing.Point(8, 20);
            this.lblChanPassToken.Name = "lblChanPassToken";
            this.lblChanPassToken.Size = new System.Drawing.Size(41, 13);
            this.lblChanPassToken.TabIndex = 0;
            this.lblChanPassToken.Text = "Token:";
            // 
            // lblChanPassPIN
            // 
            this.lblChanPassPIN.AutoSize = true;
            this.lblChanPassPIN.Location = new System.Drawing.Point(8, 48);
            this.lblChanPassPIN.Name = "lblChanPassPIN";
            this.lblChanPassPIN.Size = new System.Drawing.Size(28, 13);
            this.lblChanPassPIN.TabIndex = 2;
            this.lblChanPassPIN.Text = "PIN:";
            // 
            // txtChanPassPIN
            // 
            this.txtChanPassPIN.Location = new System.Drawing.Point(60, 44);
            this.txtChanPassPIN.Name = "txtChanPassPIN";
            this.txtChanPassPIN.Size = new System.Drawing.Size(72, 20);
            this.txtChanPassPIN.TabIndex = 3;
            this.txtChanPassPIN.UseSystemPasswordChar = true;
            // 
            // btnShowChanPassPIN
            // 
            this.btnShowChanPassPIN.Location = new System.Drawing.Point(140, 44);
            this.btnShowChanPassPIN.Name = "btnShowChanPassPIN";
            this.btnShowChanPassPIN.Size = new System.Drawing.Size(56, 23);
            this.btnShowChanPassPIN.TabIndex = 4;
            this.btnShowChanPassPIN.Text = "Show";
            this.btnShowChanPassPIN.UseVisualStyleBackColor = true;
            this.btnShowChanPassPIN.Click += new System.EventHandler(this.btnShowChanPassPIN_Click);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(424, 455);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ST4 Image Dumper";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.picCaptcha)).EndInit();
            this.grpCaptcha.ResumeLayout(false);
            this.grpCaptcha.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tpSettings.ResumeLayout(false);
            this.tpPost.ResumeLayout(false);
            this.tpComment.ResumeLayout(false);
            this.tpComment.PerformLayout();
            this.grpPosting.ResumeLayout(false);
            this.grpPosting.PerformLayout();
            this.grpUser.ResumeLayout(false);
            this.grpUser.PerformLayout();
            this.grpChanPass.ResumeLayout(false);
            this.grpChanPass.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnSubmitCaptcha;
        private System.Windows.Forms.TextBox txtCaptcha;
        private System.Windows.Forms.Button btnRefreshCaptcha;
        private System.Windows.Forms.PictureBox picCaptcha;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.GroupBox grpCaptcha;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.CheckBox chkNumberPosts;
        private System.Windows.Forms.TextBox txtPostNumberStart;
        private System.Windows.Forms.Timer tmrWaitCountdown;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.ComboBox cboInterval;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.TabPage tpPost;
        private System.Windows.Forms.LinkLabel llFileName;
        private System.Windows.Forms.Label lblFileNumber;
        private System.Windows.Forms.Label lblFileLabel;
        private System.Windows.Forms.Label lblStatusLabel;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtResultLog;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.CheckBox chkRandomizeOrder;
        private System.Windows.Forms.CheckBox chkMovePostedToSubfolder;
        private System.Windows.Forms.TabPage tpComment;
        private System.Windows.Forms.Button btnEditCommentDone;
        private System.Windows.Forms.Button btnEditComment;
        private System.Windows.Forms.Label lblIncludeComment;
        private System.Windows.Forms.Label lblIncludeCommentLabel;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Button btnShowPassword;
        private System.Windows.Forms.Label lblCaptchaExpirationWarning;
        private System.Windows.Forms.GroupBox grpPosting;
        private System.Windows.Forms.GroupBox grpChanPass;
        private System.Windows.Forms.TextBox txtChanPassToken;
        private System.Windows.Forms.Label lblChanPassToken;
        private System.Windows.Forms.Label lblChanPassPIN;
        private System.Windows.Forms.TextBox txtChanPassPIN;
        private System.Windows.Forms.GroupBox grpUser;
        private System.Windows.Forms.Button btnShowChanPassPIN;
    }
}