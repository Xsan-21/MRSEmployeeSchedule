namespace MRSES.Windows.Forms
{
    partial class FormFeedBack
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
            this.LabelInstructionInFormFeedBack = new System.Windows.Forms.Label();
            this.TextBoxUserFeedbackInFormFeedBack = new System.Windows.Forms.TextBox();
            this.LabelMessageInFormFeedBack = new System.Windows.Forms.Label();
            this.ButtonSendFeedbackInFormFeedBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelInstructionInFormFeedBack
            // 
            this.LabelInstructionInFormFeedBack.Location = new System.Drawing.Point(52, 9);
            this.LabelInstructionInFormFeedBack.Name = "LabelInstructionInFormFeedBack";
            this.LabelInstructionInFormFeedBack.Size = new System.Drawing.Size(519, 72);
            this.LabelInstructionInFormFeedBack.TabIndex = 5;
            this.LabelInstructionInFormFeedBack.Text = "Utilice esta parte para enviar alguna sugerencia que mejore su experiencia con el" +
    " programa.";
            this.LabelInstructionInFormFeedBack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBoxUserFeedbackInFormFeedBack
            // 
            this.TextBoxUserFeedbackInFormFeedBack.ForeColor = System.Drawing.Color.Gray;
            this.TextBoxUserFeedbackInFormFeedBack.Location = new System.Drawing.Point(57, 97);
            this.TextBoxUserFeedbackInFormFeedBack.Multiline = true;
            this.TextBoxUserFeedbackInFormFeedBack.Name = "TextBoxUserFeedbackInFormFeedBack";
            this.TextBoxUserFeedbackInFormFeedBack.Size = new System.Drawing.Size(502, 532);
            this.TextBoxUserFeedbackInFormFeedBack.TabIndex = 299;
            this.TextBoxUserFeedbackInFormFeedBack.Tag = "Escriba su sugerencia aquí...";
            this.TextBoxUserFeedbackInFormFeedBack.Text = "Escriba su sugerencia aquí...";
            // 
            // LabelMessageInFormFeedBack
            // 
            this.LabelMessageInFormFeedBack.BackColor = System.Drawing.Color.DarkKhaki;
            this.LabelMessageInFormFeedBack.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LabelMessageInFormFeedBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelMessageInFormFeedBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMessageInFormFeedBack.ForeColor = System.Drawing.Color.White;
            this.LabelMessageInFormFeedBack.Location = new System.Drawing.Point(0, 746);
            this.LabelMessageInFormFeedBack.Name = "LabelMessageInFormFeedBack";
            this.LabelMessageInFormFeedBack.Size = new System.Drawing.Size(619, 41);
            this.LabelMessageInFormFeedBack.TabIndex = 300;
            this.LabelMessageInFormFeedBack.Text = "Messages";
            // 
            // ButtonSendFeedbackInFormFeedBack
            // 
            this.ButtonSendFeedbackInFormFeedBack.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ButtonSendFeedbackInFormFeedBack.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.ButtonSendFeedbackInFormFeedBack.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.ButtonSendFeedbackInFormFeedBack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.ButtonSendFeedbackInFormFeedBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.ButtonSendFeedbackInFormFeedBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSendFeedbackInFormFeedBack.Location = new System.Drawing.Point(180, 651);
            this.ButtonSendFeedbackInFormFeedBack.Name = "ButtonSendFeedbackInFormFeedBack";
            this.ButtonSendFeedbackInFormFeedBack.Size = new System.Drawing.Size(250, 75);
            this.ButtonSendFeedbackInFormFeedBack.TabIndex = 301;
            this.ButtonSendFeedbackInFormFeedBack.Text = "Enviar";
            this.ButtonSendFeedbackInFormFeedBack.UseVisualStyleBackColor = true;
            this.ButtonSendFeedbackInFormFeedBack.Click += new System.EventHandler(this.ButtonSendFeedbackInFormFeedBack_Click);
            // 
            // FormFeedBack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(619, 787);
            this.Controls.Add(this.ButtonSendFeedbackInFormFeedBack);
            this.Controls.Add(this.LabelMessageInFormFeedBack);
            this.Controls.Add(this.TextBoxUserFeedbackInFormFeedBack);
            this.Controls.Add(this.LabelInstructionInFormFeedBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFeedBack";
            this.Text = "Sugerencias";
            this.Load += new System.EventHandler(this.FormFeedBack_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelInstructionInFormFeedBack;
        private System.Windows.Forms.TextBox TextBoxUserFeedbackInFormFeedBack;
        private System.Windows.Forms.Label LabelMessageInFormFeedBack;
        private System.Windows.Forms.Button ButtonSendFeedbackInFormFeedBack;
    }
}