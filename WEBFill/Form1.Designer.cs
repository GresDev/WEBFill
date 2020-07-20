namespace WEBFill
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
            this.setAuthorsButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // captchaTextBox
            // 
            this.captchaTextBox.Location = new System.Drawing.Point(26, 159);
            this.captchaTextBox.Name = "captchaTextBox";
            this.captchaTextBox.Size = new System.Drawing.Size(300, 20);
            this.captchaTextBox.TabIndex = 1;
            // 
            // authButton
            // 
            this.authButton.Location = new System.Drawing.Point(26, 194);
            this.authButton.Name = "authButton";
            this.authButton.Size = new System.Drawing.Size(142, 23);
            this.authButton.TabIndex = 1;
            this.authButton.Text = "Подключиться";
            this.authButton.UseVisualStyleBackColor = true;
            this.authButton.Click += new System.EventHandler(this.authButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Введите код с картинки:";
            // 
            // loadFromExcelButton
            // 
            this.loadFromExcelButton.Location = new System.Drawing.Point(26, 241);
            this.loadFromExcelButton.Name = "loadFromExcelButton";
            this.loadFromExcelButton.Size = new System.Drawing.Size(142, 23);
            this.loadFromExcelButton.TabIndex = 5;
            this.loadFromExcelButton.Text = "Открыть таблицу Excel";
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
            this.webBrowserGTRF.Location = new System.Drawing.Point(362, 31);
            this.webBrowserGTRF.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserGTRF.Name = "webBrowserGTRF";
            this.webBrowserGTRF.Size = new System.Drawing.Size(20, 20);
            this.webBrowserGTRF.TabIndex = 0;
            this.webBrowserGTRF.Visible = false;
            // 
            // startSendingButton
            // 
            this.startSendingButton.Enabled = false;
            this.startSendingButton.Location = new System.Drawing.Point(184, 241);
            this.startSendingButton.Name = "startSendingButton";
            this.startSendingButton.Size = new System.Drawing.Size(142, 23);
            this.startSendingButton.TabIndex = 6;
            this.startSendingButton.Text = "Начать передачу";
            this.startSendingButton.UseVisualStyleBackColor = true;
            this.startSendingButton.Click += new System.EventHandler(this.StartSendingButton_Click);
            // 
            // labelTransmitCount
            // 
            this.labelTransmitCount.AutoSize = true;
            this.labelTransmitCount.Location = new System.Drawing.Point(27, 341);
            this.labelTransmitCount.Name = "labelTransmitCount";
            this.labelTransmitCount.Size = new System.Drawing.Size(93, 13);
            this.labelTransmitCount.TabIndex = 7;
            this.labelTransmitCount.Text = "Передано 0 из 0.";
            // 
            // labelBroadcastToSend
            // 
            this.labelBroadcastToSend.AutoSize = true;
            this.labelBroadcastToSend.Location = new System.Drawing.Point(27, 377);
            this.labelBroadcastToSend.Name = "labelBroadcastToSend";
            this.labelBroadcastToSend.Size = new System.Drawing.Size(62, 13);
            this.labelBroadcastToSend.TabIndex = 8;
            this.labelBroadcastToSend.Text = "Передача: ";
            // 
            // createBroadcastTableButton
            // 
            this.createBroadcastTableButton.Enabled = false;
            this.createBroadcastTableButton.Location = new System.Drawing.Point(26, 288);
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
            this.progressBar1.Location = new System.Drawing.Point(26, 398);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 10;
            // 
            // setAuthorsButton
            // 
            this.setAuthorsButton.Enabled = false;
            this.setAuthorsButton.Location = new System.Drawing.Point(184, 288);
            this.setAuthorsButton.Name = "setAuthorsButton";
            this.setAuthorsButton.Size = new System.Drawing.Size(142, 23);
            this.setAuthorsButton.TabIndex = 11;
            this.setAuthorsButton.Text = "Заполнить авторов";
            this.setAuthorsButton.UseVisualStyleBackColor = true;
            this.setAuthorsButton.Click += new System.EventHandler(this.SetAuthorsButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(26, 31);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 90);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(184, 194);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "Перезагрузить картинку";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 492);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.setAuthorsButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.createBroadcastTableButton);
            this.Controls.Add(this.labelBroadcastToSend);
            this.Controls.Add(this.labelTransmitCount);
            this.Controls.Add(this.startSendingButton);
            this.Controls.Add(this.webBrowserGTRF);
            this.Controls.Add(this.loadFromExcelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.authButton);
            this.Controls.Add(this.captchaTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.Button setAuthorsButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
    }
}

