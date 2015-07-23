using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core.Persistence;

namespace MRSES.Windows.Forms
{
    public partial class FormSelectStore : Form
    {
        IEmployeeRepository _employeeRepository;

        public FormSelectStore()
        {
            InitializeComponent();
            LabelCurrentStoreBeingUse.Text = "Tienda actual: " + Core.Configuration.StoreLocation;
            _employeeRepository = new EmployeeRepository();
        }

        private async void FormConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                var stores = await _employeeRepository.GetStoresAsync();
                ComboBoxSelectStore.DataSource = stores;
            }
            catch (Exception ex)
            {
                Core.Shared.AlertUser.Message("No se pudo establecer conexion con la base de datos.\n" + ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            }
        }

        void SetCurrentStoreInLabel(string store)
        {
            LabelCurrentStoreBeingUse.Text = "Tienda actual: " + store;
        }

        private void ButtonSaveConfiguration_Click(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    SaveStore();
                    break;
                }
                catch (Exception ex)
                {
                    var alert = Core.Shared.AlertUser.Message("No se pudo cambiar de tienda!\n" + ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    if (alert == System.Windows.Forms.DialogResult.Abort)
                        break;
                } 
            }
        }

        void SaveStore()
        {
            string store = "";

            if (string.IsNullOrEmpty(TextBoxStoreLocation.Text))
            {
                store = ComboBoxSelectStore.Text;
            }
            else
            {
                store = TextBoxStoreLocation.Text;
            }

            var result = Core.Shared.AlertUser.Message("La tienda seleccionada es " + store + ", ¿Es correcto?", "Cambio de tienda", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.OK)
            {
                Core.Configuration.StoreLocation = store;
                RestartProgram();
            }
        }

        void RestartProgram()
        {
            try
            {
                var question = Core.Shared.AlertUser.Message("Se debe reiniciar el programa para realizar el cambio de tienda. ¿Desea reiniciar ahora?", "Reinicio requerido", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                if (question == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\MRSES.Windows.exe");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }               
            }
            catch
            { }
        }

        private void FormSelectStore_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrEmpty(Core.Configuration.StoreLocation))
            {
                var alert = Core.Shared.AlertUser.Message("Para utilizar el programa debe seleccionar su tienda.","Seleccione tienda", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                
                if(alert == System.Windows.Forms.DialogResult.Retry)
                    e.Cancel = true;
                else
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
