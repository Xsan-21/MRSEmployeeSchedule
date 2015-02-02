namespace MRSES.Windows.Forms
{
    partial class FormPrintSchedule
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
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ComboBoxPositionSelectorInFormPrintSchedule = new System.Windows.Forms.ComboBox();
            this.ComboBoxWeekSelectorInFormPrintSchedule = new System.Windows.Forms.ComboBox();
            this.TextBoxReportNameInFormPrintSchedule = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSaveInTabPageEmployeeInformation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(891, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(204, 25);
            this.label9.TabIndex = 15;
            this.label9.Text = "Seleccione posición";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(212, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(200, 25);
            this.label8.TabIndex = 14;
            this.label8.Text = "Seleccione semana";
            // 
            // ComboBoxPositionSelectorInFormPrintSchedule
            // 
            this.ComboBoxPositionSelectorInFormPrintSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ComboBoxPositionSelectorInFormPrintSchedule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxPositionSelectorInFormPrintSchedule.FormattingEnabled = true;
            this.ComboBoxPositionSelectorInFormPrintSchedule.Location = new System.Drawing.Point(698, 80);
            this.ComboBoxPositionSelectorInFormPrintSchedule.Name = "ComboBoxPositionSelectorInFormPrintSchedule";
            this.ComboBoxPositionSelectorInFormPrintSchedule.Size = new System.Drawing.Size(528, 33);
            this.ComboBoxPositionSelectorInFormPrintSchedule.TabIndex = 2;
            this.ComboBoxPositionSelectorInFormPrintSchedule.TabStop = false;
            this.ComboBoxPositionSelectorInFormPrintSchedule.SelectedValueChanged += new System.EventHandler(this.ComboBoxesInFormPrintScheduleSelectedValueChanged);
            // 
            // ComboBoxWeekSelectorInFormPrintSchedule
            // 
            this.ComboBoxWeekSelectorInFormPrintSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ComboBoxWeekSelectorInFormPrintSchedule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxWeekSelectorInFormPrintSchedule.FormattingEnabled = true;
            this.ComboBoxWeekSelectorInFormPrintSchedule.Location = new System.Drawing.Point(46, 80);
            this.ComboBoxWeekSelectorInFormPrintSchedule.Name = "ComboBoxWeekSelectorInFormPrintSchedule";
            this.ComboBoxWeekSelectorInFormPrintSchedule.Size = new System.Drawing.Size(528, 33);
            this.ComboBoxWeekSelectorInFormPrintSchedule.TabIndex = 1;
            this.ComboBoxWeekSelectorInFormPrintSchedule.TabStop = false;
            this.ComboBoxWeekSelectorInFormPrintSchedule.SelectedValueChanged += new System.EventHandler(this.ComboBoxesInFormPrintScheduleSelectedValueChanged);
            // 
            // TextBoxReportNameInFormPrintSchedule
            // 
            this.TextBoxReportNameInFormPrintSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxReportNameInFormPrintSchedule.ForeColor = System.Drawing.Color.Black;
            this.TextBoxReportNameInFormPrintSchedule.Location = new System.Drawing.Point(437, 225);
            this.TextBoxReportNameInFormPrintSchedule.Name = "TextBoxReportNameInFormPrintSchedule";
            this.TextBoxReportNameInFormPrintSchedule.Size = new System.Drawing.Size(370, 31);
            this.TextBoxReportNameInFormPrintSchedule.TabIndex = 3;
            this.TextBoxReportNameInFormPrintSchedule.TabStop = false;
            this.TextBoxReportNameInFormPrintSchedule.Tag = "Nombre de documento";
            this.TextBoxReportNameInFormPrintSchedule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(690, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 25);
            this.label1.TabIndex = 17;
            this.label1.Text = "(opcional)";
            // 
            // ButtonSaveInTabPageEmployeeInformation
            // 
            this.ButtonSaveInTabPageEmployeeInformation.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.ButtonSaveInTabPageEmployeeInformation.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.ButtonSaveInTabPageEmployeeInformation.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.ButtonSaveInTabPageEmployeeInformation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.ButtonSaveInTabPageEmployeeInformation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSaveInTabPageEmployeeInformation.Location = new System.Drawing.Point(495, 312);
            this.ButtonSaveInTabPageEmployeeInformation.Name = "ButtonSaveInTabPageEmployeeInformation";
            this.ButtonSaveInTabPageEmployeeInformation.Size = new System.Drawing.Size(250, 75);
            this.ButtonSaveInTabPageEmployeeInformation.TabIndex = 4;
            this.ButtonSaveInTabPageEmployeeInformation.Text = "Imprimir horario";
            this.ButtonSaveInTabPageEmployeeInformation.UseVisualStyleBackColor = true;
            this.ButtonSaveInTabPageEmployeeInformation.Click += new System.EventHandler(this.ButtonSaveInTabPageEmployeeInformation_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(444, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 25);
            this.label2.TabIndex = 19;
            this.label2.Text = "Nombre del documento:";
            // 
            // FormPrintSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1272, 414);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ButtonSaveInTabPageEmployeeInformation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxReportNameInFormPrintSchedule);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ComboBoxPositionSelectorInFormPrintSchedule);
            this.Controls.Add(this.ComboBoxWeekSelectorInFormPrintSchedule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormPrintSchedule";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Horarios";
            this.Load += new System.EventHandler(this.FormPrintSchedule_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox ComboBoxPositionSelectorInFormPrintSchedule;
        private System.Windows.Forms.ComboBox ComboBoxWeekSelectorInFormPrintSchedule;
        private System.Windows.Forms.TextBox TextBoxReportNameInFormPrintSchedule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSaveInTabPageEmployeeInformation;
        private System.Windows.Forms.Label label2;
    }
}