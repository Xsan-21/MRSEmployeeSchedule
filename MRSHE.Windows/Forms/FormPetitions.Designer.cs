namespace MRSES.Windows.Forms
{
    partial class FormPetitions
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
            this.RichTextBoxPetitionViewInFormPetitions = new System.Windows.Forms.RichTextBox();
            this.ComboBoxPositionSelectorInFormPetition = new System.Windows.Forms.ComboBox();
            this.ComboBoxWeekSelectorInFormPetition = new System.Windows.Forms.ComboBox();
            this.LabelMessageResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RichTextBoxPetitionViewInFormPetitions
            // 
            this.RichTextBoxPetitionViewInFormPetitions.BackColor = System.Drawing.Color.White;
            this.RichTextBoxPetitionViewInFormPetitions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RichTextBoxPetitionViewInFormPetitions.Enabled = false;
            this.RichTextBoxPetitionViewInFormPetitions.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextBoxPetitionViewInFormPetitions.Location = new System.Drawing.Point(0, 108);
            this.RichTextBoxPetitionViewInFormPetitions.Name = "RichTextBoxPetitionViewInFormPetitions";
            this.RichTextBoxPetitionViewInFormPetitions.ReadOnly = true;
            this.RichTextBoxPetitionViewInFormPetitions.Size = new System.Drawing.Size(1081, 523);
            this.RichTextBoxPetitionViewInFormPetitions.TabIndex = 0;
            this.RichTextBoxPetitionViewInFormPetitions.TabStop = false;
            this.RichTextBoxPetitionViewInFormPetitions.Text = "";
            // 
            // ComboBoxPositionSelectorInFormPetition
            // 
            this.ComboBoxPositionSelectorInFormPetition.FormattingEnabled = true;
            this.ComboBoxPositionSelectorInFormPetition.Location = new System.Drawing.Point(617, 11);
            this.ComboBoxPositionSelectorInFormPetition.Name = "ComboBoxPositionSelectorInFormPetition";
            this.ComboBoxPositionSelectorInFormPetition.Size = new System.Drawing.Size(342, 33);
            this.ComboBoxPositionSelectorInFormPetition.TabIndex = 2;
            this.ComboBoxPositionSelectorInFormPetition.Text = "Seleccione posición";
            this.ComboBoxPositionSelectorInFormPetition.SelectedValueChanged += new System.EventHandler(this.SelectedValueChangedInComboBoxWeekOrPositionAsync);
            // 
            // ComboBoxWeekSelectorInFormPetition
            // 
            this.ComboBoxWeekSelectorInFormPetition.FormattingEnabled = true;
            this.ComboBoxWeekSelectorInFormPetition.Location = new System.Drawing.Point(150, 11);
            this.ComboBoxWeekSelectorInFormPetition.Name = "ComboBoxWeekSelectorInFormPetition";
            this.ComboBoxWeekSelectorInFormPetition.Size = new System.Drawing.Size(342, 33);
            this.ComboBoxWeekSelectorInFormPetition.TabIndex = 1;
            this.ComboBoxWeekSelectorInFormPetition.Text = "Seleccione semana";
            this.ComboBoxWeekSelectorInFormPetition.SelectedValueChanged += new System.EventHandler(this.SelectedValueChangedInComboBoxWeekOrPositionAsync);
            // 
            // LabelMessageResult
            // 
            this.LabelMessageResult.AutoSize = true;
            this.LabelMessageResult.BackColor = System.Drawing.SystemColors.Control;
            this.LabelMessageResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelMessageResult.ForeColor = System.Drawing.Color.DarkGreen;
            this.LabelMessageResult.Location = new System.Drawing.Point(256, 338);
            this.LabelMessageResult.Name = "LabelMessageResult";
            this.LabelMessageResult.Size = new System.Drawing.Size(445, 37);
            this.LabelMessageResult.TabIndex = 8;
            this.LabelMessageResult.Text = "Seleccione semana y posición";
            // 
            // FormPetitions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1081, 631);
            this.Controls.Add(this.LabelMessageResult);
            this.Controls.Add(this.RichTextBoxPetitionViewInFormPetitions);
            this.Controls.Add(this.ComboBoxPositionSelectorInFormPetition);
            this.Controls.Add(this.ComboBoxWeekSelectorInFormPetition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPetitions";
            this.Text = "Peticiones de Empleados";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPetitions_FormClosing);
            this.Load += new System.EventHandler(this.FormPetitions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RichTextBoxPetitionViewInFormPetitions;
        private System.Windows.Forms.ComboBox ComboBoxPositionSelectorInFormPetition;
        private System.Windows.Forms.ComboBox ComboBoxWeekSelectorInFormPetition;
        private System.Windows.Forms.Label LabelMessageResult;
    }
}