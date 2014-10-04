namespace MRSES.Windows.Forms
{
    partial class FormAbout
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
            this.LabelTitleInAboutForm = new System.Windows.Forms.Label();
            this.LabelFormAboutInformation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LabelTitleInAboutForm
            // 
            this.LabelTitleInAboutForm.BackColor = System.Drawing.Color.White;
            this.LabelTitleInAboutForm.Dock = System.Windows.Forms.DockStyle.Top;
            this.LabelTitleInAboutForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTitleInAboutForm.Location = new System.Drawing.Point(0, 0);
            this.LabelTitleInAboutForm.Name = "LabelTitleInAboutForm";
            this.LabelTitleInAboutForm.Size = new System.Drawing.Size(1383, 94);
            this.LabelTitleInAboutForm.TabIndex = 2;
            this.LabelTitleInAboutForm.Text = "MRSES";
            this.LabelTitleInAboutForm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelFormAboutInformation
            // 
            this.LabelFormAboutInformation.AutoSize = true;
            this.LabelFormAboutInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelFormAboutInformation.Location = new System.Drawing.Point(12, 131);
            this.LabelFormAboutInformation.Name = "LabelFormAboutInformation";
            this.LabelFormAboutInformation.Size = new System.Drawing.Size(219, 55);
            this.LabelFormAboutInformation.TabIndex = 3;
            this.LabelFormAboutInformation.Text = "infoLabel";
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1383, 633);
            this.Controls.Add(this.LabelFormAboutInformation);
            this.Controls.Add(this.LabelTitleInAboutForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.Text = "Acerca de MRSES";
            this.Load += new System.EventHandler(this.FormAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelTitleInAboutForm;
        private System.Windows.Forms.Label LabelFormAboutInformation;
    }
}