namespace NCRQuality_Inspection
{
    partial class LoadingWin
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
            this.lblTaskOnProcess = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTaskOnProcess
            // 
            this.lblTaskOnProcess.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTaskOnProcess.AutoSize = true;
            this.lblTaskOnProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTaskOnProcess.ForeColor = System.Drawing.Color.Black;
            this.lblTaskOnProcess.Location = new System.Drawing.Point(13, 19);
            this.lblTaskOnProcess.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTaskOnProcess.Name = "lblTaskOnProcess";
            this.lblTaskOnProcess.Size = new System.Drawing.Size(287, 39);
            this.lblTaskOnProcess.TabIndex = 64;
            this.lblTaskOnProcess.Text = "PLEASE WAIT...";
            // 
            // LoadingWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(29)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(800, 185);
            this.Controls.Add(this.lblTaskOnProcess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoadingWin";
            this.Opacity = 0.85D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoadingWin";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTaskOnProcess;
    }
}