namespace MRSES.Windows.Forms
{
    partial class FormSelectStore
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
            this.ComboBoxSelectStore = new System.Windows.Forms.ComboBox();
            this.ButtonSaveConfiguration = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBoxStoreLocation = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seleccione su tienda:";
            // 
            // ComboBoxSelectStore
            // 
            this.ComboBoxSelectStore.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxSelectStore.FormattingEnabled = true;
            this.ComboBoxSelectStore.Location = new System.Drawing.Point(222, 103);
            this.ComboBoxSelectStore.Name = "ComboBoxSelectStore";
            this.ComboBoxSelectStore.Size = new System.Drawing.Size(254, 33);
            this.ComboBoxSelectStore.TabIndex = 1;
            // 
            // ButtonSaveConfiguration
            // 
            this.ButtonSaveConfiguration.AutoSize = true;
            this.ButtonSaveConfiguration.FlatAppearance.BorderColor = System.Drawing.Color.DarkKhaki;
            this.ButtonSaveConfiguration.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
            this.ButtonSaveConfiguration.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.ButtonSaveConfiguration.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.ButtonSaveConfiguration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSaveConfiguration.Location = new System.Drawing.Point(222, 351);
            this.ButtonSaveConfiguration.Name = "ButtonSaveConfiguration";
            this.ButtonSaveConfiguration.Size = new System.Drawing.Size(250, 75);
            this.ButtonSaveConfiguration.TabIndex = 10;
            this.ButtonSaveConfiguration.Text = "Guardar tienda";
            this.ButtonSaveConfiguration.UseVisualStyleBackColor = true;
            this.ButtonSaveConfiguration.Click += new System.EventHandler(this.ButtonSaveConfiguration_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(119, 190);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(471, 25);
            this.label3.TabIndex = 34;
            this.label3.Text = "Si no aparece, escribala en el siguiente espacio\r\n";
            // 
            // TextBoxStoreLocation
            // 
            this.TextBoxStoreLocation.Location = new System.Drawing.Point(222, 257);
            this.TextBoxStoreLocation.MaxLength = 100;
            this.TextBoxStoreLocation.Name = "TextBoxStoreLocation";
            this.TextBoxStoreLocation.Size = new System.Drawing.Size(254, 31);
            this.TextBoxStoreLocation.TabIndex = 35;
            // 
            // FormSelectStore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(729, 453);
            this.Controls.Add(this.TextBoxStoreLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ButtonSaveConfiguration);
            this.Controls.Add(this.ComboBoxSelectStore);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormSelectStore";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración";
            this.Load += new System.EventHandler(this.FormConfiguration_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxSelectStore;
        private System.Windows.Forms.Button ButtonSaveConfiguration;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBoxStoreLocation;
    }
}