namespace WindowsFormsApp1
{
    partial class chat
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
            this.recorder1 = new System.Windows.Forms.PictureBox();
            this.txtrec = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.opener = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lstChat = new System.Windows.Forms.ListBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.recorder1)).BeginInit();
            this.SuspendLayout();
            // 
            // recorder1
            // 
            this.recorder1.Image = global::WindowsFormsApp1.Properties.Resources.microphone;
            this.recorder1.Location = new System.Drawing.Point(233, 432);
            this.recorder1.Name = "recorder1";
            this.recorder1.Size = new System.Drawing.Size(30, 24);
            this.recorder1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.recorder1.TabIndex = 1;
            this.recorder1.TabStop = false;
            this.recorder1.Click += new System.EventHandler(this.recorder1_Click);
            // 
            // txtrec
            // 
            this.txtrec.AutoSize = true;
            this.txtrec.Location = new System.Drawing.Point(214, 415);
            this.txtrec.Name = "txtrec";
            this.txtrec.Size = new System.Drawing.Size(74, 13);
            this.txtrec.TabIndex = 2;
            this.txtrec.Text = "start recording";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(3, 436);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(224, 20);
            this.txtMessage.TabIndex = 3;
            // 
            // opener
            // 
            this.opener.Location = new System.Drawing.Point(12, 411);
            this.opener.Name = "opener";
            this.opener.Size = new System.Drawing.Size(53, 20);
            this.opener.TabIndex = 4;
            this.opener.Text = "...";
            this.opener.UseVisualStyleBackColor = true;
            this.opener.Click += new System.EventHandler(this.opener_Click);
            // 
            // exit
            // 
            this.exit.AutoSize = true;
            this.exit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exit.ForeColor = System.Drawing.Color.Red;
            this.exit.Location = new System.Drawing.Point(327, 9);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(17, 15);
            this.exit.TabIndex = 5;
            this.exit.Text = "X";
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.button1.Location = new System.Drawing.Point(294, 427);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 30);
            this.button1.TabIndex = 6;
            this.button1.Text = "XOR";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // lstChat
            // 
            this.lstChat.FormattingEnabled = true;
            this.lstChat.Location = new System.Drawing.Point(3, 38);
            this.lstChat.Name = "lstChat";
            this.lstChat.Size = new System.Drawing.Size(349, 355);
            this.lstChat.TabIndex = 7;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(158, 11);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(35, 13);
            this.lblUser.TabIndex = 9;
            this.lblUser.Text = "label1";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(71, 411);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(70, 20);
            this.btnSend.TabIndex = 10;
            this.btnSend.Text = "send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // chat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 469);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lstChat);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.opener);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtrec);
            this.Controls.Add(this.recorder1);
            this.Name = "chat";
            this.Text = "chat";
            ((System.ComponentModel.ISupportInitialize)(this.recorder1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox recorder1;
        private System.Windows.Forms.Label txtrec;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button opener;
        private System.Windows.Forms.Label exit;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lstChat;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Button btnSend;
    }
}