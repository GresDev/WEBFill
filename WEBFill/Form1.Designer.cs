﻿namespace WEBFill
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.captchaTextBox = new System.Windows.Forms.TextBox();
            this.authButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.loadFromExcelButton = new System.Windows.Forms.Button();
            this.openExcelFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.webBrowserGTRF = new System.Windows.Forms.WebBrowser();
            this.startSendingButton = new System.Windows.Forms.Button();
            this.labelTransmitCount = new System.Windows.Forms.Label();
            this.labelBroadcastToSend = new System.Windows.Forms.Label();
            this.createBroadcastTableButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.captchaPictureBox = new System.Windows.Forms.PictureBox();
            this.reloadCaptchaButton = new System.Windows.Forms.Button();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.captchaGroupBox = new System.Windows.Forms.GroupBox();
            this.authGroupBox = new System.Windows.Forms.GroupBox();
            this.userPasswordTextBox = new System.Windows.Forms.TextBox();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.openExcelFileLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.captchaPictureBox)).BeginInit();
            this.captchaGroupBox.SuspendLayout();
            this.authGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // captchaTextBox
            // 
            this.captchaTextBox.Location = new System.Drawing.Point(14, 111);
            this.captchaTextBox.Name = "captchaTextBox";
            this.captchaTextBox.Size = new System.Drawing.Size(227, 20);
            this.captchaTextBox.TabIndex = 1;
            this.captchaTextBox.TextChanged += new System.EventHandler(this.captchaTextBox_TextChanged);
            // 
            // authButton
            // 
            this.authButton.Enabled = false;
            this.authButton.Location = new System.Drawing.Point(14, 145);
            this.authButton.Name = "authButton";
            this.authButton.Size = new System.Drawing.Size(227, 23);
            this.authButton.TabIndex = 1;
            this.authButton.Text = "Подключиться";
            this.authButton.UseVisualStyleBackColor = true;
            this.authButton.Click += new System.EventHandler(this.authButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Код с картинки";
            // 
            // loadFromExcelButton
            // 
            this.loadFromExcelButton.Location = new System.Drawing.Point(450, 31);
            this.loadFromExcelButton.Name = "loadFromExcelButton";
            this.loadFromExcelButton.Size = new System.Drawing.Size(37, 23);
            this.loadFromExcelButton.TabIndex = 5;
            this.loadFromExcelButton.Text = "...";
            this.loadFromExcelButton.UseVisualStyleBackColor = true;
            this.loadFromExcelButton.Click += new System.EventHandler(this.loadFromExcelButton_Click);
            // 
            // openExcelFileDialog
            // 
            this.openExcelFileDialog.Filter = "Excel files|*.xls*";
            // 
            // webBrowserGTRF
            // 
            this.webBrowserGTRF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowserGTRF.Location = new System.Drawing.Point(106, 30);
            this.webBrowserGTRF.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserGTRF.Name = "webBrowserGTRF";
            this.webBrowserGTRF.Size = new System.Drawing.Size(42, 40);
            this.webBrowserGTRF.TabIndex = 0;
            this.webBrowserGTRF.Visible = false;
            // 
            // startSendingButton
            // 
            this.startSendingButton.Enabled = false;
            this.startSendingButton.Location = new System.Drawing.Point(298, 225);
            this.startSendingButton.Name = "startSendingButton";
            this.startSendingButton.Size = new System.Drawing.Size(142, 23);
            this.startSendingButton.TabIndex = 6;
            this.startSendingButton.Text = "Начать передачу";
            this.startSendingButton.UseVisualStyleBackColor = true;
            this.startSendingButton.Click += new System.EventHandler(this.StartSendingButton_Click);
            // 
            // labelTransmitCount
            // 
            this.labelTransmitCount.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTransmitCount.AutoSize = true;
            this.labelTransmitCount.Location = new System.Drawing.Point(536, 389);
            this.labelTransmitCount.Name = "labelTransmitCount";
            this.labelTransmitCount.Size = new System.Drawing.Size(24, 13);
            this.labelTransmitCount.TabIndex = 7;
            this.labelTransmitCount.Text = "0/0";
            this.labelTransmitCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelBroadcastToSend
            // 
            this.labelBroadcastToSend.AutoSize = true;
            this.labelBroadcastToSend.Location = new System.Drawing.Point(295, 359);
            this.labelBroadcastToSend.Name = "labelBroadcastToSend";
            this.labelBroadcastToSend.Size = new System.Drawing.Size(62, 13);
            this.labelBroadcastToSend.TabIndex = 8;
            this.labelBroadcastToSend.Text = "Передача: ";
            // 
            // createBroadcastTableButton
            // 
            this.createBroadcastTableButton.Enabled = false;
            this.createBroadcastTableButton.Location = new System.Drawing.Point(298, 97);
            this.createBroadcastTableButton.Name = "createBroadcastTableButton";
            this.createBroadcastTableButton.Size = new System.Drawing.Size(142, 23);
            this.createBroadcastTableButton.TabIndex = 9;
            this.createBroadcastTableButton.Text = "Заполнить таблицу";
            this.createBroadcastTableButton.UseVisualStyleBackColor = true;
            this.createBroadcastTableButton.Click += new System.EventHandler(this.CreateBroadcastTableButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(12, 384);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1073, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 10;
            // 
            // captchaPictureBox
            // 
            this.captchaPictureBox.Location = new System.Drawing.Point(14, 19);
            this.captchaPictureBox.Name = "captchaPictureBox";
            this.captchaPictureBox.Size = new System.Drawing.Size(227, 62);
            this.captchaPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.captchaPictureBox.TabIndex = 12;
            this.captchaPictureBox.TabStop = false;
            // 
            // reloadCaptchaButton
            // 
            this.reloadCaptchaButton.Location = new System.Drawing.Point(14, 148);
            this.reloadCaptchaButton.Name = "reloadCaptchaButton";
            this.reloadCaptchaButton.Size = new System.Drawing.Size(227, 23);
            this.reloadCaptchaButton.TabIndex = 13;
            this.reloadCaptchaButton.Text = "Перезагрузить картинку";
            this.reloadCaptchaButton.UseVisualStyleBackColor = true;
            this.reloadCaptchaButton.Click += new System.EventHandler(this.reloadCaptchaButton_Click);
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Location = new System.Drawing.Point(15, 31);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(103, 13);
            this.userNameLabel.TabIndex = 14;
            this.userNameLabel.Text = "Имя пользователя";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(15, 84);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(45, 13);
            this.passwordLabel.TabIndex = 15;
            this.passwordLabel.Text = "Пароль";
            // 
            // captchaGroupBox
            // 
            this.captchaGroupBox.Controls.Add(this.webBrowserGTRF);
            this.captchaGroupBox.Controls.Add(this.captchaTextBox);
            this.captchaGroupBox.Controls.Add(this.label1);
            this.captchaGroupBox.Controls.Add(this.reloadCaptchaButton);
            this.captchaGroupBox.Controls.Add(this.captchaPictureBox);
            this.captchaGroupBox.Location = new System.Drawing.Point(12, 12);
            this.captchaGroupBox.Name = "captchaGroupBox";
            this.captchaGroupBox.Size = new System.Drawing.Size(256, 186);
            this.captchaGroupBox.TabIndex = 16;
            this.captchaGroupBox.TabStop = false;
            // 
            // authGroupBox
            // 
            this.authGroupBox.Controls.Add(this.userPasswordTextBox);
            this.authGroupBox.Controls.Add(this.userNameTextBox);
            this.authGroupBox.Controls.Add(this.userNameLabel);
            this.authGroupBox.Controls.Add(this.passwordLabel);
            this.authGroupBox.Controls.Add(this.authButton);
            this.authGroupBox.Location = new System.Drawing.Point(12, 204);
            this.authGroupBox.Name = "authGroupBox";
            this.authGroupBox.Size = new System.Drawing.Size(256, 174);
            this.authGroupBox.TabIndex = 17;
            this.authGroupBox.TabStop = false;
            this.authGroupBox.Text = "Учётные данные";
            // 
            // userPasswordTextBox
            // 
            this.userPasswordTextBox.Location = new System.Drawing.Point(14, 100);
            this.userPasswordTextBox.MaxLength = 32;
            this.userPasswordTextBox.Name = "userPasswordTextBox";
            this.userPasswordTextBox.PasswordChar = '*';
            this.userPasswordTextBox.Size = new System.Drawing.Size(227, 20);
            this.userPasswordTextBox.TabIndex = 17;
            this.userPasswordTextBox.TextChanged += new System.EventHandler(this.userPasswordTextBox_TextChanged);
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(14, 47);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(227, 20);
            this.userNameTextBox.TabIndex = 16;
            this.userNameTextBox.TextChanged += new System.EventHandler(this.userNameTextBox_TextChanged);
            // 
            // openExcelFileLabel
            // 
            this.openExcelFileLabel.AutoSize = true;
            this.openExcelFileLabel.Location = new System.Drawing.Point(295, 36);
            this.openExcelFileLabel.Name = "openExcelFileLabel";
            this.openExcelFileLabel.Size = new System.Drawing.Size(141, 13);
            this.openExcelFileLabel.TabIndex = 18;
            this.openExcelFileLabel.Text = "Открыть таблицу передач:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1097, 528);
            this.Controls.Add(this.openExcelFileLabel);
            this.Controls.Add(this.labelTransmitCount);
            this.Controls.Add(this.authGroupBox);
            this.Controls.Add(this.captchaGroupBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.createBroadcastTableButton);
            this.Controls.Add(this.labelBroadcastToSend);
            this.Controls.Add(this.startSendingButton);
            this.Controls.Add(this.loadFromExcelButton);
            this.Name = "Form1";
            this.Text = "BroadSend";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.captchaPictureBox)).EndInit();
            this.captchaGroupBox.ResumeLayout(false);
            this.captchaGroupBox.PerformLayout();
            this.authGroupBox.ResumeLayout(false);
            this.authGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox captchaTextBox;
        private System.Windows.Forms.Button authButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button loadFromExcelButton;
        private System.Windows.Forms.OpenFileDialog openExcelFileDialog;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.WebBrowser webBrowserGTRF;
        private System.Windows.Forms.Button startSendingButton;
        private System.Windows.Forms.Label labelTransmitCount;
        private System.Windows.Forms.Label labelBroadcastToSend;
        private System.Windows.Forms.Button createBroadcastTableButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox captchaPictureBox;
        private System.Windows.Forms.Button reloadCaptchaButton;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.GroupBox captchaGroupBox;
        private System.Windows.Forms.GroupBox authGroupBox;
        private System.Windows.Forms.TextBox userPasswordTextBox;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label openExcelFileLabel;
    }
}

