using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;

namespace MRSES.Windows.Forms
{
    public partial class FormConfigureBusiness : Form
    {
        IBusinessRepository _businessRepo;
        List<Business> Businesses { get; set; }
        List<Location> Locations { get; set; }
        bool Cancel { get; set; }

        string SelectedBusiness { get { return StringIsNullOrEmpty(TextBoxBusiness.Text) ? ComboBoxtBusinesses.Text : TextBoxBusiness.Text; } }
        string SelectedLocation { get { return StringIsNullOrEmpty(TextBoxCity.Text) ? ComboBoxCities.Text : TextBoxCity.Text; } }
        string SelectedFirstDayOfWeek { get { return Core.Shared.DateFunctions.FirstDayOfWeek(ComboBoxFirstDayOfWeek.Text).ToString(); } }
        string SelectedReportFolderLocation { get { return TextBoxReportLocationFolder.Text; } }
        string SelectedPhone { get { return TextBoxPhoneNumber.Text; } }
        string SelectedAccessKey { get { return TextBoxAccessKey.Text; } set { TextBoxAccessKey.Text = value; } }

        public FormConfigureBusiness()
        {
            InitializeComponent();
            var _business = Configuration.Business + ", " + Configuration.Location;
            LabelCurrentStoreBeingUse.Text = "Empresa actual: " + (_business.Length == 2 ? "No asignada" : _business);
            ComboBoxFirstDayOfWeek.DataSource = new[] { "miércoles", "jueves", "viernes", "sábado", "domingo", "lunes", "martes" };

            ComboBoxtBusinesses.Text = Configuration.Business;
            ComboBoxCities.Text = Configuration.Location;

            TextBoxPhoneNumber.Text = Configuration.Phone;
            TextBoxReportLocationFolder.Text = Configuration.ReportFolderLocation;
        }

        private async void FormConfigureAppSettings_Load(object sender, EventArgs e)
        {
            try
            {
                await FillComboBoxBusinessesAsync();
                await FillComboBoxCitiesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo establecer conexion con la base de datos.\nDetalles: " + ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            }
        }

        async Task FillComboBoxBusinessesAsync()
        {
            using (_businessRepo = new BusinessRepository())
            {
                if (Businesses == null)
                {
                    Businesses = await _businessRepo.GetBusinessesAsync();

                    if (Businesses.Count > 0)
                        ComboBoxtBusinesses.DataSource = Businesses.Select(n => n.Name).ToList();
                }             
            }
        }

        async Task FillComboBoxCitiesAsync()
        {
            using (_businessRepo = new BusinessRepository())
            {
               
                Locations = await _businessRepo.GetBusinessLocationsAsync(SelectedBusiness);

                if(Locations.Count > 0)
                {
                    ComboBoxCities.DataSource = Locations.Select(n => n.City).ToList();
                    ComboBoxFirstDayOfWeek.SelectedIndex = WeekDayIndex(Businesses.Where(b => b.Name == SelectedBusiness).Single().FirstDayOfWeek);
                }
            }
        }

        private async void ButtonSaveConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                await SaveBusinessInformationAsync();
            }
            catch (Exception ex)
            {
                var alert = MessageBox.Show("No se pudo guardar o cambiar de localización!" + ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
        }

        async Task SaveBusinessInformationAsync()
        {
            if (!RequiredInformationIsComplete())
            {
                ShowMessageToUser();
                return;
            }

            var result = MessageBox.Show("La empresa seleccionada es " + SelectedBusiness + " de " + SelectedLocation + ", ¿Es correcto?", "Cambio de tienda", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

            if (result == DialogResult.OK)
                await SaveAsync();
        }

        int WeekDayIndex(string day)
        {
            int index = 0;
            switch (day)
            {
                case "Wednesday":
                    break;
                case "Thursday":
                    index = 1;
                    break;
                case "Friday":
                    index = 2;
                    break;
                case "Saturday":
                    index = 3;
                    break;
                case "Sunday":
                    index = 4;
                    break;
                case "Monday":
                    index = 5;
                    break;
                case "Tuesday":
                    index = 6;
                    break;
                default:
                    break;
            }

            return index;
        }

        async Task SaveAsync()
        {

            using (_businessRepo = new BusinessRepository())
            {
                try
                {
                    _businessRepo.Business = GetSelectedBusiness();
                    _businessRepo.Location = GetSelectedLocation();
                    await _businessRepo.SaveAsync();
                }
                catch (Exception)
                {
                    throw new Exception("\n\nPosibles razones:\n1) Llave de acceso no válida 2) No se logró comunicar con la base de datos.");
                }
            }

            SetBusinessInConfiguration();

            if (!Configuration.IsNewInstallation)
                RestartProgram();
            else
                Close();
        }

        Business GetSelectedBusiness()
        {
            var selectedBusiness = new Business(SelectedBusiness, SelectedFirstDayOfWeek);
            if(Businesses.Any(b => b.Name == SelectedBusiness))
                selectedBusiness.ObjectId = Businesses.Where(b => b.Name == SelectedBusiness).Single().ObjectId;

            return selectedBusiness;
        }

        Location GetSelectedLocation()
        {
            var selectedLocation = new Location(SelectedAccessKey, SelectedLocation, SelectedPhone);

            if(Locations.Any(l => l.AccessKey == SelectedAccessKey))
            {
                var info = Locations.Where(l => l.AccessKey == SelectedAccessKey).Select(o => new { AccessKey = o.AccessKey, ObjectID = o.ObjectId }).Single();
                selectedLocation.ObjectId = info.ObjectID;
                selectedLocation.AccessKey = info.AccessKey;
            }

            return selectedLocation;
        }

        void SetBusinessInConfiguration()
        {
            Configuration.Business = SelectedBusiness;
            Configuration.Location = SelectedLocation;
            Configuration.FirstDayOfWeek = SelectedFirstDayOfWeek;
            Configuration.ReportFolderLocation = SelectedReportFolderLocation;
            Configuration.AccessKey = SelectedAccessKey;
            Configuration.Phone = SelectedPhone;
        }

        bool RequiredInformationIsComplete()
        {
            if (StringIsNullOrEmpty(SelectedBusiness))
            {
                return false;
            }
            else if (StringIsNullOrEmpty(SelectedLocation))
            {
                return false;
            }
            else if (StringIsNullOrEmpty(SelectedReportFolderLocation))
            {
                return false;
            }
            else if (StringIsNullOrEmpty(SelectedPhone))
            {
                return false;
            }
            else if (!StringIsNullOrEmpty(SelectedReportFolderLocation))
            {
                if (!System.IO.Directory.Exists(SelectedReportFolderLocation))
                {
                    // TODO use one method for showing messages
                    MessageBox.Show("El archivo seleccionado no existe, favor seleccione uno válido.", "Archivo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return false;
                }
            }

            
            if (!StringIsNullOrEmpty(TextBoxBusiness.Text))
            {
                if (ComboBoxtBusinesses.Items.Contains(SelectedBusiness))
                {
                    MessageBox.Show("El negocio que escribió ya existe, favor seleccionelo de la primera lista.", "Negocio existe", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    ComboBoxtBusinesses.Text = "";
                    return false;
                }
            }

            
            if (!StringIsNullOrEmpty(TextBoxCity.Text))
            {
                if (ComboBoxCities.Items.Contains(SelectedLocation))
                {
                    MessageBox.Show("La localización que escribió ya existe, favor seleccionela de la segunda lista.", "Localización ya existe", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    ComboBoxCities.Text = "";
                    return false; 
                }
            }

            if (StringIsNullOrEmpty(SelectedAccessKey) || SelectedAccessKey.Length != 17 || SelectedAccessKey.Count(c => c == '-') != 2)
            {
                MessageBox.Show("La llave de acceso no es válida.", "Llave de acceso inválida", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                TextBoxAccessKey.Focus();
                return false;
            }
            else if (StringIsNullOrEmpty(TextBoxBusiness.Text) && StringIsNullOrEmpty(TextBoxCity.Text))
            {
                if(!Locations.Any(l => l.AccessKey == SelectedAccessKey))
                {
                    MessageBox.Show("Debe indicar la llave de acceso para la localización selecionada.", "Llave de acceso incorrecta", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    TextBoxAccessKey.Focus();
                    return false;
                }
            }
            
            return true;
        }

        bool StringIsNullOrEmpty(string text)
        {
            return string.IsNullOrEmpty(text);
        }

        void RestartProgram()
        {
            try
            {
                var question = MessageBox.Show("Se debe reiniciar el programa para aplicar los cambios aplicados. ¿Desea reiniciar ahora?", "Reinicio requerido", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                if (question == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\MRSES.Windows.exe");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            catch
            { }
        }

        void ShowMessageToUser()
        {
            // TODO use one method for showing messages
            var alert = MessageBox.Show("Por favor complete los datos solicitados.", "Información incompleta", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

            if (StringIsNullOrEmpty(SelectedAccessKey) || SelectedAccessKey.Length != 17 || SelectedAccessKey.Count(c => c == '-') != 2)
                return;

            if (alert == DialogResult.Cancel)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            else
                Cancel = true; // means that user selected Retry and the program should not close.
        }

        private void ButtonSelectReportFolder_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            DialogResult dialog = folderDialog.ShowDialog();

            if (dialog == DialogResult.OK)
            {
                TextBoxReportLocationFolder.Text = folderDialog.SelectedPath;
            }
        }

        private async void PutLocationsOfBusinessInComboBoxSelectLocations(object sender, EventArgs e)
        {
            await FillComboBoxCitiesAsync();
        }

        private void PutLocationInfoInTextBoxes(object sender, EventArgs e)
        {
            var _locationInfo = Locations.Where(l => l.City == SelectedLocation).Single();
            TextBoxPhoneNumber.Text = _locationInfo.PhoneNumber;
        }

        private void GenerateAccessKey(object sender, EventArgs e)
        {
            if (!StringIsNullOrEmpty(TextBoxBusiness.Text) || !StringIsNullOrEmpty(TextBoxCity.Text))
                TextBoxAccessKey.Text = Core.Shared.StringFunctions.GenerateObjectId(15, true);
            else
                TextBoxAccessKey.Clear();
        }

        private void FormConfigureBusiness_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Configuration.SettingsAreNotAssigned())
                ShowMessageToUser();

            e.Cancel = Cancel; // if Cancel is true, that means that program closing event should be cancelled. And will continue running.
            Cancel = false;
        }
    }
}
