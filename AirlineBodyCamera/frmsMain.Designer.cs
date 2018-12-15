namespace AirlineBodyCamera
{
    partial class frmsMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmsMain));
            this.skinPictureBox1 = new CCWin.SkinControl.SkinPictureBox();
            this.skinPictureBox2 = new CCWin.SkinControl.SkinPictureBox();
            this.skinGroupBox1 = new CCWin.SkinControl.SkinGroupBox();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.scomboUserID = new CCWin.SkinControl.SkinComboBox();
            this.stxtDevicePassword = new CCWin.SkinControl.SkinTextBox();
            this.sbtnCheckDevice = new CCWin.SkinControl.SkinButton();
            this.sbtnLogin = new CCWin.SkinControl.SkinButton();
            this.sbtnExit = new CCWin.SkinControl.SkinButton();
            this.sbtnRestart = new CCWin.SkinControl.SkinButton();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox2)).BeginInit();
            this.skinGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // skinPictureBox1
            // 
            this.skinPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinPictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("skinPictureBox1.Image")));
            this.skinPictureBox1.Location = new System.Drawing.Point(79, 45);
            this.skinPictureBox1.Name = "skinPictureBox1";
            this.skinPictureBox1.Size = new System.Drawing.Size(295, 71);
            this.skinPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.skinPictureBox1.TabIndex = 0;
            this.skinPictureBox1.TabStop = false;
            // 
            // skinPictureBox2
            // 
            this.skinPictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.skinPictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("skinPictureBox2.Image")));
            this.skinPictureBox2.Location = new System.Drawing.Point(378, 44);
            this.skinPictureBox2.Name = "skinPictureBox2";
            this.skinPictureBox2.Size = new System.Drawing.Size(420, 83);
            this.skinPictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.skinPictureBox2.TabIndex = 1;
            this.skinPictureBox2.TabStop = false;
            // 
            // skinGroupBox1
            // 
            this.skinGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox1.BorderColor = System.Drawing.Color.Red;
            this.skinGroupBox1.Controls.Add(this.sbtnExit);
            this.skinGroupBox1.Controls.Add(this.sbtnRestart);
            this.skinGroupBox1.Controls.Add(this.sbtnLogin);
            this.skinGroupBox1.Controls.Add(this.sbtnCheckDevice);
            this.skinGroupBox1.Controls.Add(this.stxtDevicePassword);
            this.skinGroupBox1.Controls.Add(this.scomboUserID);
            this.skinGroupBox1.Controls.Add(this.skinLabel2);
            this.skinGroupBox1.Controls.Add(this.skinLabel1);
            this.skinGroupBox1.ForeColor = System.Drawing.Color.Blue;
            this.skinGroupBox1.Location = new System.Drawing.Point(23, 128);
            this.skinGroupBox1.Name = "skinGroupBox1";
            this.skinGroupBox1.RectBackColor = System.Drawing.Color.White;
            this.skinGroupBox1.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox1.Size = new System.Drawing.Size(296, 79);
            this.skinGroupBox1.TabIndex = 2;
            this.skinGroupBox1.TabStop = false;
            this.skinGroupBox1.Text = "登录信息";
            this.skinGroupBox1.TitleBorderColor = System.Drawing.Color.Red;
            this.skinGroupBox1.TitleRectBackColor = System.Drawing.Color.White;
            this.skinGroupBox1.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // skinLabel1
            // 
            this.skinLabel1.AutoSize = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel1.Location = new System.Drawing.Point(6, 17);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.Size = new System.Drawing.Size(56, 17);
            this.skinLabel1.TabIndex = 3;
            this.skinLabel1.Text = "账号类型";
            // 
            // skinLabel2
            // 
            this.skinLabel2.AutoSize = true;
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.Location = new System.Drawing.Point(6, 44);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(56, 17);
            this.skinLabel2.TabIndex = 4;
            this.skinLabel2.Text = "登录密码";
            // 
            // scomboUserID
            // 
            this.scomboUserID.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.scomboUserID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scomboUserID.FormattingEnabled = true;
            this.scomboUserID.Items.AddRange(new object[] {
            "管理员",
            "一般用户"});
            this.scomboUserID.Location = new System.Drawing.Point(64, 13);
            this.scomboUserID.Name = "scomboUserID";
            this.scomboUserID.Size = new System.Drawing.Size(90, 22);
            this.scomboUserID.TabIndex = 3;
            this.scomboUserID.WaterText = "";
            // 
            // stxtDevicePassword
            // 
            this.stxtDevicePassword.BackColor = System.Drawing.Color.Transparent;
            this.stxtDevicePassword.DownBack = null;
            this.stxtDevicePassword.Icon = null;
            this.stxtDevicePassword.IconIsButton = false;
            this.stxtDevicePassword.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.stxtDevicePassword.IsPasswordChat = '*';
            this.stxtDevicePassword.IsSystemPasswordChar = false;
            this.stxtDevicePassword.Lines = new string[0];
            this.stxtDevicePassword.Location = new System.Drawing.Point(63, 38);
            this.stxtDevicePassword.Margin = new System.Windows.Forms.Padding(0);
            this.stxtDevicePassword.MaxLength = 32767;
            this.stxtDevicePassword.MinimumSize = new System.Drawing.Size(28, 28);
            this.stxtDevicePassword.MouseBack = null;
            this.stxtDevicePassword.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.stxtDevicePassword.Multiline = false;
            this.stxtDevicePassword.Name = "stxtDevicePassword";
            this.stxtDevicePassword.NormlBack = null;
            this.stxtDevicePassword.Padding = new System.Windows.Forms.Padding(5);
            this.stxtDevicePassword.ReadOnly = false;
            this.stxtDevicePassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.stxtDevicePassword.Size = new System.Drawing.Size(91, 28);
            // 
            // 
            // 
            this.stxtDevicePassword.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stxtDevicePassword.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stxtDevicePassword.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.stxtDevicePassword.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.stxtDevicePassword.SkinTxt.Name = "BaseText";
            this.stxtDevicePassword.SkinTxt.PasswordChar = '*';
            this.stxtDevicePassword.SkinTxt.Size = new System.Drawing.Size(81, 18);
            this.stxtDevicePassword.SkinTxt.TabIndex = 0;
            this.stxtDevicePassword.SkinTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.stxtDevicePassword.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.stxtDevicePassword.SkinTxt.WaterText = "";
            this.stxtDevicePassword.TabIndex = 3;
            this.stxtDevicePassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.stxtDevicePassword.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.stxtDevicePassword.WaterText = "";
            this.stxtDevicePassword.WordWrap = true;
            // 
            // sbtnCheckDevice
            // 
            this.sbtnCheckDevice.BackColor = System.Drawing.Color.Transparent;
            this.sbtnCheckDevice.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.sbtnCheckDevice.DownBack = null;
            this.sbtnCheckDevice.Location = new System.Drawing.Point(160, 12);
            this.sbtnCheckDevice.MouseBack = null;
            this.sbtnCheckDevice.Name = "sbtnCheckDevice";
            this.sbtnCheckDevice.NormlBack = null;
            this.sbtnCheckDevice.Size = new System.Drawing.Size(61, 29);
            this.sbtnCheckDevice.TabIndex = 3;
            this.sbtnCheckDevice.Text = "检查设备";
            this.sbtnCheckDevice.UseVisualStyleBackColor = false;
            // 
            // sbtnLogin
            // 
            this.sbtnLogin.BackColor = System.Drawing.Color.Transparent;
            this.sbtnLogin.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.sbtnLogin.DownBack = null;
            this.sbtnLogin.Location = new System.Drawing.Point(160, 44);
            this.sbtnLogin.MouseBack = null;
            this.sbtnLogin.Name = "sbtnLogin";
            this.sbtnLogin.NormlBack = null;
            this.sbtnLogin.Size = new System.Drawing.Size(61, 29);
            this.sbtnLogin.TabIndex = 5;
            this.sbtnLogin.Text = "登录";
            this.sbtnLogin.UseVisualStyleBackColor = false;
            // 
            // sbtnExit
            // 
            this.sbtnExit.BackColor = System.Drawing.Color.Transparent;
            this.sbtnExit.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.sbtnExit.DownBack = null;
            this.sbtnExit.Location = new System.Drawing.Point(227, 44);
            this.sbtnExit.MouseBack = null;
            this.sbtnExit.Name = "sbtnExit";
            this.sbtnExit.NormlBack = null;
            this.sbtnExit.Size = new System.Drawing.Size(61, 29);
            this.sbtnExit.TabIndex = 7;
            this.sbtnExit.Text = "退出";
            this.sbtnExit.UseVisualStyleBackColor = false;
            // 
            // sbtnRestart
            // 
            this.sbtnRestart.BackColor = System.Drawing.Color.Transparent;
            this.sbtnRestart.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.sbtnRestart.DownBack = null;
            this.sbtnRestart.Location = new System.Drawing.Point(227, 12);
            this.sbtnRestart.MouseBack = null;
            this.sbtnRestart.Name = "sbtnRestart";
            this.sbtnRestart.NormlBack = null;
            this.sbtnRestart.Size = new System.Drawing.Size(61, 29);
            this.sbtnRestart.TabIndex = 6;
            this.sbtnRestart.Text = "重启软件";
            this.sbtnRestart.UseVisualStyleBackColor = false;
            // 
            // frmsMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(951, 689);
            this.Controls.Add(this.skinGroupBox1);
            this.Controls.Add(this.skinPictureBox2);
            this.Controls.Add(this.skinPictureBox1);
            this.Name = "frmsMain";
            this.Text = "frmsMain";
            this.Load += new System.EventHandler(this.frmsMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox2)).EndInit();
            this.skinGroupBox1.ResumeLayout(false);
            this.skinGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinPictureBox skinPictureBox1;
        private CCWin.SkinControl.SkinPictureBox skinPictureBox2;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox1;
        private CCWin.SkinControl.SkinLabel skinLabel1;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private CCWin.SkinControl.SkinComboBox scomboUserID;
        private CCWin.SkinControl.SkinTextBox stxtDevicePassword;
        private CCWin.SkinControl.SkinButton sbtnExit;
        private CCWin.SkinControl.SkinButton sbtnRestart;
        private CCWin.SkinControl.SkinButton sbtnLogin;
        private CCWin.SkinControl.SkinButton sbtnCheckDevice;

    }
}