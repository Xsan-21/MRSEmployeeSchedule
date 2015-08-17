namespace MRSES.Windows.Forms
{
    partial class FormConfigureBusiness
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
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxCities = new System.Windows.Forms.ComboBox();
            this.ButtonSaveConfiguration = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxCity = new System.Windows.Forms.TextBox();
            this.LabelCurrentStoreBeingUse = new System.Windows.Forms.Label();
            this.TextBoxBusiness = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboBoxtBusinesses = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.TextBoxReportLocationFolder = new System.Windows.Forms.TextBox();
            this.ButtonSelectReportFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.ComboBoxFirstDayOfWeek = new System.Windows.Forms.ComboBox();
            this.TextBoxPhoneNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TextBoxAccessKey = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(276, 300);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(322, 36);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seleccione su localización:";
            // 
            // ComboBoxCities
            // 
            this.ComboBoxCities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCities.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxCities.FormattingEnabled = true;
            this.ComboBoxCities.Location = new System.Drawing.Point(628, 299);
            this.ComboBoxCities.Name = "ComboBoxCities";
            this.ComboBoxCities.Size = new System.Drawing.Size(355, 41);
            this.ComboBoxCities.TabIndex = 3;
            this.ComboBoxCities.SelectedIndexChanged += new System.EventHandler(this.PutLocationInfoInTextBoxes);
            // 
            // ButtonSaveConfiguration
            // 
            this.ButtonSaveConfiguration.AutoSize = true;
            this.ButtonSaveConfiguration.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.ButtonSaveConfiguration.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.ButtonSaveConfiguration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.ButtonSaveConfiguration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.ButtonSaveConfiguration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSaveConfiguration.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonSaveConfiguration.Location = new System.Drawing.Point(488, 961);
            this.ButtonSaveConfiguration.Name = "ButtonSaveConfiguration";
            this.ButtonSaveConfiguration.Size = new System.Drawing.Size(250, 75);
            this.ButtonSaveConfiguration.TabIndex = 9;
            this.ButtonSaveConfiguration.Text = "Guardar localización";
            this.ButtonSaveConfiguration.UseVisualStyleBackColor = true;
            this.ButtonSaveConfiguration.Click += new System.EventHandler(this.ButtonSaveConfiguration_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(163, 400);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(408, 29);
            this.label3.TabIndex = 34;
            this.label3.Text = "No aparece su localidad? Escribala aquí:";
            // 
            // TextBoxCity
            // 
            this.TextBoxCity.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxCity.Location = new System.Drawing.Point(628, 394);
            this.TextBoxCity.MaxLength = 100;
            this.TextBoxCity.Name = "TextBoxCity";
            this.TextBoxCity.Size = new System.Drawing.Size(355, 40);
            this.TextBoxCity.TabIndex = 4;
            this.TextBoxCity.TextChanged += new System.EventHandler(this.GenerateAccessKey);
            // 
            // LabelCurrentStoreBeingUse
            // 
            this.LabelCurrentStoreBeingUse.AutoSize = true;
            this.LabelCurrentStoreBeingUse.Font = new System.Drawing.Font("Calibri", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelCurrentStoreBeingUse.Location = new System.Drawing.Point(315, 22);
            this.LabelCurrentStoreBeingUse.Name = "LabelCurrentStoreBeingUse";
            this.LabelCurrentStoreBeingUse.Size = new System.Drawing.Size(506, 45);
            this.LabelCurrentStoreBeingUse.TabIndex = 36;
            this.LabelCurrentStoreBeingUse.Text = "Empresa actual: No identificada";
            // 
            // TextBoxBusiness
            // 
            this.TextBoxBusiness.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxBusiness.Location = new System.Drawing.Point(625, 206);
            this.TextBoxBusiness.MaxLength = 100;
            this.TextBoxBusiness.Name = "TextBoxBusiness";
            this.TextBoxBusiness.Size = new System.Drawing.Size(355, 40);
            this.TextBoxBusiness.TabIndex = 2;
            this.TextBoxBusiness.TextChanged += new System.EventHandler(this.GenerateAccessKey);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(169, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(402, 29);
            this.label2.TabIndex = 39;
            this.label2.Text = "No aparece su empresa? Escribala aquí:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ComboBoxtBusinesses
            // 
            this.ComboBoxtBusinesses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxtBusinesses.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxtBusinesses.FormattingEnabled = true;
            this.ComboBoxtBusinesses.Location = new System.Drawing.Point(625, 112);
            this.ComboBoxtBusinesses.Name = "ComboBoxtBusinesses";
            this.ComboBoxtBusinesses.Size = new System.Drawing.Size(355, 41);
            this.ComboBoxtBusinesses.TabIndex = 1;
            this.ComboBoxtBusinesses.SelectedIndexChanged += new System.EventHandler(this.PutLocationsOfBusinessInComboBoxSelectLocations);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(311, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(287, 36);
            this.label4.TabIndex = 37;
            this.label4.Text = "Seleccione su empresa:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(58, 672);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(540, 36);
            this.label5.TabIndex = 40;
            this.label5.Text = "Seleccione archivo para guardar los horarios:";
            // 
            // TextBoxReportLocationFolder
            // 
            this.TextBoxReportLocationFolder.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxReportLocationFolder.Location = new System.Drawing.Point(633, 672);
            this.TextBoxReportLocationFolder.MaxLength = 100;
            this.TextBoxReportLocationFolder.Name = "TextBoxReportLocationFolder";
            this.TextBoxReportLocationFolder.Size = new System.Drawing.Size(350, 40);
            this.TextBoxReportLocationFolder.TabIndex = 7;
            // 
            // ButtonSelectReportFolder
            // 
            this.ButtonSelectReportFolder.AutoSize = true;
            this.ButtonSelectReportFolder.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.ButtonSelectReportFolder.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.ButtonSelectReportFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.ButtonSelectReportFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.ButtonSelectReportFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSelectReportFolder.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonSelectReportFolder.Location = new System.Drawing.Point(1017, 665);
            this.ButtonSelectReportFolder.Name = "ButtonSelectReportFolder";
            this.ButtonSelectReportFolder.Size = new System.Drawing.Size(221, 53);
            this.ButtonSelectReportFolder.TabIndex = 0;
            this.ButtonSelectReportFolder.TabStop = false;
            this.ButtonSelectReportFolder.Text = "Buscar archivo";
            this.ButtonSelectReportFolder.UseVisualStyleBackColor = true;
            this.ButtonSelectReportFolder.Click += new System.EventHandler(this.ButtonSelectReportFolder_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(34, 489);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(564, 36);
            this.label6.TabIndex = 43;
            this.label6.Text = "Seleccione primer dia de trabajo en la semana:";
            // 
            // ComboBoxFirstDayOfWeek
            // 
            this.ComboBoxFirstDayOfWeek.AutoCompleteCustomSource.AddRange(new string[] {
            "lunes",
            "martes",
            "miércoles",
            "jueves",
            "viernes",
            "sábado",
            "domingo"});
            this.ComboBoxFirstDayOfWeek.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBoxFirstDayOfWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxFirstDayOfWeek.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxFirstDayOfWeek.FormattingEnabled = true;
            this.ComboBoxFirstDayOfWeek.ItemHeight = 33;
            this.ComboBoxFirstDayOfWeek.Items.AddRange(new object[] {
            "martes",
            "miércoles",
            "jueves",
            "viernes",
            "sábado",
            "domingo",
            "lunes"});
            this.ComboBoxFirstDayOfWeek.Location = new System.Drawing.Point(633, 488);
            this.ComboBoxFirstDayOfWeek.Name = "ComboBoxFirstDayOfWeek";
            this.ComboBoxFirstDayOfWeek.Size = new System.Drawing.Size(350, 41);
            this.ComboBoxFirstDayOfWeek.TabIndex = 5;
            // 
            // TextBoxPhoneNumber
            // 
            this.TextBoxPhoneNumber.Font = new System.Drawing.Font("Calibri", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxPhoneNumber.Location = new System.Drawing.Point(630, 577);
            this.TextBoxPhoneNumber.MaxLength = 100;
            this.TextBoxPhoneNumber.Name = "TextBoxPhoneNumber";
            this.TextBoxPhoneNumber.Size = new System.Drawing.Size(350, 40);
            this.TextBoxPhoneNumber.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(309, 578);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(289, 36);
            this.label7.TabIndex = 45;
            this.label7.Text = "Número de teléfono(s):";
            // 
            // TextBoxAccessKey
            // 
            this.TextBoxAccessKey.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBoxAccessKey.Font = new System.Drawing.Font("Calibri", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBoxAccessKey.Location = new System.Drawing.Point(367, 840);
            this.TextBoxAccessKey.MaxLength = 17;
            this.TextBoxAccessKey.Name = "TextBoxAccessKey";
            this.TextBoxAccessKey.Size = new System.Drawing.Size(506, 53);
            this.TextBoxAccessKey.TabIndex = 8;
            this.TextBoxAccessKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(508, 770);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(211, 36);
            this.label8.TabIndex = 47;
            this.label8.Text = "Llave de acceso:";
            // 
            // FormConfigureBusiness
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1268, 1084);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TextBoxAccessKey);
            this.Controls.Add(this.TextBoxPhoneNumber);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ComboBoxFirstDayOfWeek);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ButtonSelectReportFolder);
            this.Controls.Add(this.TextBoxReportLocationFolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TextBoxBusiness);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ComboBoxtBusinesses);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.LabelCurrentStoreBeingUse);
            this.Controls.Add(this.TextBoxCity);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ButtonSaveConfiguration);
            this.Controls.Add(this.ComboBoxCities);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormConfigureBusiness";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigureBusiness_FormClosing);
            this.Load += new System.EventHandler(this.FormConfigureAppSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxCities;
        private System.Windows.Forms.Button ButtonSaveConfiguration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxCity;
        private System.Windows.Forms.Label LabelCurrentStoreBeingUse;
        private System.Windows.Forms.TextBox TextBoxBusiness;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ComboBoxtBusinesses;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TextBoxReportLocationFolder;
        private System.Windows.Forms.Button ButtonSelectReportFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox ComboBoxFirstDayOfWeek;
        private System.Windows.Forms.TextBox TextBoxPhoneNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextBoxAccessKey;
        private System.Windows.Forms.Label label8;
    }
}