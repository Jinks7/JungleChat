namespace Jungle_Chat
{
    partial class Request
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
            this.lblMain = new System.Windows.Forms.Label();
            this.txtMain = new System.Windows.Forms.TextBox();
            this.lblSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMain
            // 
            this.lblMain.AutoSize = true;
            this.lblMain.Location = new System.Drawing.Point(12, 12);
            this.lblMain.Name = "lblMain";
            this.lblMain.Size = new System.Drawing.Size(42, 17);
            this.lblMain.TabIndex = 0;
            this.lblMain.Text = "Join: ";
            // 
            // txtMain
            // 
            this.txtMain.Location = new System.Drawing.Point(64, 9);
            this.txtMain.MaxLength = 20;
            this.txtMain.Name = "txtMain";
            this.txtMain.Size = new System.Drawing.Size(150, 22);
            this.txtMain.TabIndex = 1;
            this.txtMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMain_KeyDown);
            // 
            // lblSend
            // 
            this.lblSend.Location = new System.Drawing.Point(137, 45);
            this.lblSend.Name = "lblSend";
            this.lblSend.Size = new System.Drawing.Size(75, 31);
            this.lblSend.TabIndex = 2;
            this.lblSend.Text = "Send";
            this.lblSend.UseVisualStyleBackColor = true;
            this.lblSend.Click += new System.EventHandler(this.button1_Click);
            // 
            // Request
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(224, 88);
            this.Controls.Add(this.lblSend);
            this.Controls.Add(this.txtMain);
            this.Controls.Add(this.lblMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Request";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMain;
        private System.Windows.Forms.TextBox txtMain;
        private System.Windows.Forms.Button lblSend;
    }
}