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
            _employeeRepository = new EmployeeRepository();
        }

        private async void FormConfiguration_Load(object sender, EventArgs e)
        {
            var stores = await _employeeRepository.GetStoresAsync();
            ComboBoxSelectStore.DataSource = stores;
        }

        private void ButtonSaveConfiguration_Click(object sender, EventArgs e)
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
                this.Close();
            }
        }
    }
}
