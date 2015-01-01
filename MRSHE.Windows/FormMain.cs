using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using MRSES.Core.Persistence;
using MRSES.Windows.Forms;

namespace MRSES.Windows
{
    public partial class FormMain : Form
    {
        #region Variables

        string _idOfCurrentSelectedEmployeeInEmployeeTabPage;
        System.Globalization.CultureInfo _culture = new System.Globalization.CultureInfo("en-US");
        FormPetitions formPetitions;
        EmployeeRepository employeeRepository;
        AvailabilityRepository _availabilityRepo;
        PetitionRepository _petitionRepo;
   
        #endregion

        #region Properties

        #endregion

        #region Initialization

        public FormMain()
        {
            InitializeComponent();
            this.Width = 850; this.Height = 500;
            PetitionDatePicker.MinDate = DateTime.Now.AddDays(1);

            employeeRepository = new EmployeeRepository();
            formPetitions = new FormPetitions();
            _availabilityRepo = new AvailabilityRepository();
            _petitionRepo = new PetitionRepository();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await FillPositionComboBoxAsync();
            FillWeekSelectorComboBox();
        } 

        #endregion

        #region Shared Methods For Main Form

        async Task FillPositionComboBoxAsync()
        {
            var positions = await employeeRepository.GetPositionsAsync();
            ComboBoxPositionSelectorInFormMain.DataSource = positions;
        }

        void FillWeekSelectorComboBox()
        {
            var weeks = ComboBoxFunctions.GetCurrentAndNextThreeWeeks();
            ComboBoxWeekSelectorInFormMain.DataSource = weeks.ToArray();
        }

        public async Task ShowMessageInLabelMessageOfFormMain(string message, string result)
        {
            LabelMessageInFormMain.Text = message;
            
            if(result == "error")
                LabelMessageInFormMain.BackColor = Color.Red;
            

            await Task.Delay(5000);
            LabelMessageInFormMain.BackColor = Color.DarkKhaki;
            LabelMessageInFormMain.Text = "";
        }

        string CurrentSelectedPositionInComboBox()
        {
            return ComboBoxPositionSelectorInFormMain.Text;
        }

        async Task<List<string>> GetAllEmployeeNamesByPositionAsync(string position)
        {
            return await employeeRepository.GetEmployeeNamesByPositionAsync(position);
        }

        void ValidateAndConvertHourInShortFormatToLongIfNecesary(TextBox textbox)
        {
            TimeFunctions.ChangeShortFormatToLongFormat(textbox);
        }

        #endregion        

        #region Main Form Events

        async void ComboBoxPositionSelectorInFormMainValueChanged(object sender, EventArgs e)
        {
            await FillListBoxEmployeesInEmployeeTabPageAsync();
            await FillComboBoxSelectEmployeeInScheduleTabPageAsync();
            await FillComboBoxSelectEmployeeInPetitionsTabPageAsync();
            await FillComboBoxSelectEmployeeInAvailabilityTabPageAsync();
        }

        #region Button clicks

        void ShowTabPageAccordingToClickedButton(object sender, EventArgs e)
        {
            switch ((sender as Button).Tag.ToString())
            {
                case "Ver Semana de Trabajo":
                    TapControlInFormMain.SelectedIndex = 0;
                    break;
                case "Información de Empleados":
                    TapControlInFormMain.SelectedIndex = 1;
                    break;
                case "Disponibilidad":
                    TapControlInFormMain.SelectedIndex = 2;
                    break;
                case "Peticiones":
                    TapControlInFormMain.SelectedIndex = 3;
                    break;
                case "Crear o Manejar Horarios":
                    TapControlInFormMain.SelectedIndex = 4;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Mouse hover 
        
        #endregion

        #region Textbox events

        void RemoveDefaultTextIndicatorInTextBox(object sender, EventArgs e)
        {
            StringFunctions.RemoveDefaultTextIndicator(sender as TextBox);                                        
        }

        void SetDefaultTextIndicatorInTextBox(object sender, EventArgs e)
        {
            StringFunctions.SetDefaultTextIndicatorInTextBox(sender as TextBox);
        }

        void ChangeColorOfText(object sender, EventArgs e)
        {
            StringFunctions.ChangeTextColor(sender as TextBox);
        }

        #endregion

        #region ToolStrip Menu

        private void ToolStripMenuItemAcercaDelPrograma_Click(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog();
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ToolStripMenuItemFeedBack_Click(object sender, EventArgs e)
        {
            new FormFeedBack().ShowDialog();
        }

        private void ToolStripMenuItemPetitions_Click(object sender, EventArgs e)
        {
            formPetitions.Show();
        }
        
        #endregion     

        #endregion

        #region TabPage Employee Information

        #region Events

        async void ListBoxEmployeesInEmployeeTabPageSelectedIndex(object sender, EventArgs e)
        {
            if (ListBoxEmployeesInEmployeeTabPage.SelectedIndex >= 0)
                await SendInformationToTextboxesAsync();
        }

        private async void ButtonSaveInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            try
            {
                DisableButtonsInTabPageEmployeeInformation();
                await ValidateEmployeeInformationBeforeSavingAsync();
            }
            catch(Exception ex) 
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
            finally
            {
                EnableButtonsInTabPageEmployeeInformation();
            }            
        }

        private async void ButtonDeleteInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            try
            {
                DisableButtonsInTabPageEmployeeInformation();
                await DeleteEmployeeAfterUserConfirmationAsync();
            }
            catch(Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
            finally
            {
                ClearEmployeeInformationTextBoxes();
                EnableButtonsInTabPageEmployeeInformation();
            }
        }

        private void ButtonClearTextBoxesInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            ClearEmployeeInformationTextBoxes();
        }
        
        #endregion

        #region Methods

        void EnableButtonsInTabPageEmployeeInformation()
        {
            ButtonSaveInTabPageEmployeeInformation.Enabled = true;
            ButtonDeleteInTabPageEmployeeInformation.Enabled = true;
            ButtonClearTextBoxesInTabPageEmployeeInformation.Enabled = true;
        }

        void DisableButtonsInTabPageEmployeeInformation()
        {
            ButtonSaveInTabPageEmployeeInformation.Enabled = false;
            ButtonDeleteInTabPageEmployeeInformation.Enabled = false;
            ButtonClearTextBoxesInTabPageEmployeeInformation.Enabled = false;
        }

        bool ValidateEmployeeInformationInTextBoxesOfEmployeeTabPage()
        {
            return
                TextBoxEmployeeIDInTabPageEmployeeInformation.Text == TextBoxEmployeeIDInTabPageEmployeeInformation.Tag.ToString() ? false
                : TextBoxEmployeePositionInTabPageEmployeeInformation.Text == TextBoxEmployeePositionInTabPageEmployeeInformation.Tag.ToString() ? false
                : TextBoxEmployeeNameInTabPageEmployeeInformation.Text == TextBoxEmployeeNameInTabPageEmployeeInformation.Tag.ToString() ? false
                : StringFunctions.StringIsNullOrEmpty(TextBoxEmployeeIDInTabPageEmployeeInformation.Text) ? false
                : StringFunctions.StringIsNullOrEmpty(TextBoxEmployeePositionInTabPageEmployeeInformation.Text) ? false
                : StringFunctions.StringIsNullOrEmpty(TextBoxEmployeeNameInTabPageEmployeeInformation.Text) ? false
                : true;
        }

        async Task DeleteEmployeeAfterUserConfirmationAsync()
        {
            if (ListBoxEmployeesInEmployeeTabPage.SelectedIndex < 0)
            {
                await ShowMessageInLabelMessageOfFormMain("Seleccione un empleado de la lista para eliminarlo.", "error");
                return;
            }

            var dialogResult = AlertUser.Message("¿Desea borrar el empleado " + TextBoxEmployeeNameInTabPageEmployeeInformation.Text + " del sistema?", "Eliminar empleado", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
            {
                await DeleteEmployeeInformationInTabEmployeeAsync();
                await  ShowMessageInLabelMessageOfFormMain("El empleado " + TextBoxEmployeeNameInTabPageEmployeeInformation.Text + " se eliminó.", "done");
            } 
        }

        void ClearEmployeeInformationTextBoxes()
        {
            TextBoxEmployeeNameInTabPageEmployeeInformation.Text = TextBoxEmployeeNameInTabPageEmployeeInformation.Tag.ToString();
            TextBoxEmployeeDepartmentInTabPageEmployeeInformation.Text = TextBoxEmployeeDepartmentInTabPageEmployeeInformation.Tag.ToString();
            TextBoxEmployeeIDInTabPageEmployeeInformation.Text = TextBoxEmployeeIDInTabPageEmployeeInformation.Tag.ToString();
            TextBoxEmployeePhoneNumberInTabPageEmployeeInformation.Text = TextBoxEmployeePhoneNumberInTabPageEmployeeInformation.Tag.ToString();
            TextBoxEmployeePositionInTabPageEmployeeInformation.Text = TextBoxEmployeePositionInTabPageEmployeeInformation.Tag.ToString(); ;
            CheckBoxIsStudentInTabPageEmployeeInformation.Checked = false;
            CheckBoxIsPartTimeInTabPageEmployeeInformation.Checked = false;
            CheckBoxIsFulltimeInTabPageEmployeeInformation.Checked = false;
        }

        async Task ValidateEmployeeInformationBeforeSavingAsync()
        {
            bool result = ValidateEmployeeInformationInTextBoxesOfEmployeeTabPage();

            if (result)
            {
                await SaveEmployeeInformationInTabEmployeeAsync();
                await ShowMessageInLabelMessageOfFormMain("Nuevo empleado agregado o actualizado.", "");
                await FillPositionComboBoxAsync();
            }
            else
            {
                await ShowMessageInLabelMessageOfFormMain("No ha completado la información requerida del empleado.", "error");     
            }
        }

        async Task FillListBoxEmployeesInEmployeeTabPageAsync()
        {
            string position = CurrentSelectedPositionInComboBox();
            var employeeNames = await GetAllEmployeeNamesByPositionAsync(position);
            ListBoxEmployeesInEmployeeTabPage.DataSource = employeeNames;
        }

        async Task SendInformationToTextboxesAsync()
        {
            string employeeName = ListBoxEmployeesInEmployeeTabPage.SelectedItem.ToString();
            var employeeInfo = await employeeRepository.GetEmployeeAsync(employeeName);

            _idOfCurrentSelectedEmployeeInEmployeeTabPage = employeeInfo.ID;

            TextBoxEmployeeNameInTabPageEmployeeInformation.Text = employeeInfo.Name;
            TextBoxEmployeeDepartmentInTabPageEmployeeInformation.Text = employeeInfo.Department;
            TextBoxEmployeeIDInTabPageEmployeeInformation.Text = employeeInfo.ID;
            TextBoxEmployeePhoneNumberInTabPageEmployeeInformation.Text = employeeInfo.PhoneNumber;
            TextBoxEmployeePositionInTabPageEmployeeInformation.Text = employeeInfo.Position;
            CheckBoxIsStudentInTabPageEmployeeInformation.Checked = employeeInfo.IsStudent;
            CheckBoxIsPartTimeInTabPageEmployeeInformation.Checked = employeeInfo.JobType == "Part-Time" ? true : false;
            CheckBoxIsFulltimeInTabPageEmployeeInformation.Checked = employeeInfo.JobType == "Full-Time" ? true : false;
        }

        async Task SaveEmployeeInformationInTabEmployeeAsync()
        {
            var employee = new Employee 
            {
                Name = TextBoxEmployeeNameInTabPageEmployeeInformation.Text,
                ID = TextBoxEmployeeIDInTabPageEmployeeInformation.Text,
                Department = TextBoxEmployeeDepartmentInTabPageEmployeeInformation.Text,
                IsStudent = CheckBoxIsStudentInTabPageEmployeeInformation.Checked,
                JobType = CheckBoxIsPartTimeInTabPageEmployeeInformation.Checked == true ? "Part-Time" : "Full-Time",
                PhoneNumber = TextBoxEmployeePhoneNumberInTabPageEmployeeInformation.Text,
                Position = TextBoxEmployeePositionInTabPageEmployeeInformation.Text,
                OldNameOrID = _idOfCurrentSelectedEmployeeInEmployeeTabPage
            };

            employeeRepository.Employee = employee;
            await employeeRepository.SaveAsync();
            _idOfCurrentSelectedEmployeeInEmployeeTabPage = string.Empty;
        }

        async Task DeleteEmployeeInformationInTabEmployeeAsync()
        {
            employeeRepository.Employee = new Employee() { Name = TextBoxEmployeeNameInTabPageEmployeeInformation.Text,  ID = TextBoxEmployeeIDInTabPageEmployeeInformation.Text };
            await employeeRepository.DeleteAsync();
            ListBoxEmployeesInEmployeeTabPage.Items.Remove(ListBoxEmployeesInEmployeeTabPage.SelectedItem);
        }

        #endregion         

        #endregion        

        #region TabPage Employee Availability

        #region Events

        async void SelectedIndexChangeInTextBoxEmployeeNameInAvailabilityTabPage(object sender, EventArgs e)
        {
            try
            {
                await SendEmployeeAvailabilityToTextBoxesAsync();
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        private async void ButtonSaveInTabPageAvailability_Click(object sender, EventArgs e)
        {
            try
            {
                await SaveEmployeeAvailabilityAsync();
                await ShowMessageInLabelMessageOfFormMain("Se guardo la disponibilidad de " + ComboBoxSelectEmployeeInTabPageAvailability.Text, "");
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }
        
        #endregion

        #region Methods

        Availability GetEmployeeAvailabilityFromTextBoxes()
        {
            string _wednesday = TextBoxWednesdayInTabPageAvailability.Text.Trim(),
                _thursday = TextBoxThursdayInTabPageAvailability.Text.Trim(),
                _friday = TextBoxFridayInTabPageAvailability.Text.Trim(),
                _saturday = TextBoxSaturdayInTabPageAvailability.Text.Trim(),
                _sunday = TextBoxSundayInTabPageAvailability.Text.Trim(),
                _monday = TextBoxMondayInTabPageAvailability.Text.Trim(),
                _tuesday = TextBoxTuesdayInTabPageAvailability.Text.Trim();

            var _possibleNotAvailableInputs = new List<string>() { "nd","no disponible" };

            return new Availability()
            {
                Wednesday = _wednesday.Contains('-') ? _wednesday : _possibleNotAvailableInputs.Any(p => p == _wednesday) ? "no disponible" : null,
                Thursday = _thursday.Contains('-') ? _thursday : _possibleNotAvailableInputs.Any(p => p == _thursday) ? "no disponible" : null,
                Friday = _friday.Contains('-') ? _friday : _possibleNotAvailableInputs.Any(p => p == _friday) ? "no disponible" : null,
                Saturday = _saturday.Contains('-') ? _saturday : _possibleNotAvailableInputs.Any(p => p == _saturday) ? "no disponible" : null,
                Sunday = _sunday.Contains('-') ? _sunday : _possibleNotAvailableInputs.Any(p => p == _sunday) ? "no disponible" : null,
                Monday = _monday.Contains('-') ? _monday : _possibleNotAvailableInputs.Any(p => p == _monday) ? "no disponible" : null,
                Tuesday = _tuesday.Contains('-') ? _tuesday : _possibleNotAvailableInputs.Any(p => p == _tuesday) ? "no disponible" : null
            };
        }

        async Task SaveEmployeeAvailabilityAsync()
        {
            ValidateAvailabilityHourFormatOfTextBoxes();

            _availabilityRepo.EmployeeName = ComboBoxSelectEmployeeInTabPageAvailability.Text;
            _availabilityRepo.Availability = GetEmployeeAvailabilityFromTextBoxes();
            await _availabilityRepo.SaveAsync();
        }

        async Task SendEmployeeAvailabilityToTextBoxesAsync()
        {
            _availabilityRepo.EmployeeName = ComboBoxSelectEmployeeInTabPageAvailability.Text;
            var availability = await _availabilityRepo.GetAvailabilityAsync();

            TextBoxWednesdayInTabPageAvailability.Text = availability.Wednesday;
            TextBoxThursdayInTabPageAvailability.Text = availability.Thursday;
            TextBoxFridayInTabPageAvailability.Text = availability.Friday;
            TextBoxSaturdayInTabPageAvailability.Text = availability.Saturday;
            TextBoxSundayInTabPageAvailability.Text = availability.Sunday;
            TextBoxMondayInTabPageAvailability.Text = availability.Monday;
            TextBoxTuesdayInTabPageAvailability.Text = availability.Tuesday;
        }

        void ValidateAvailabilityHourFormatOfTextBoxes()
        {
            var textBoxes = new[] { TextBoxWednesdayInTabPageAvailability, TextBoxThursdayInTabPageAvailability, TextBoxFridayInTabPageAvailability,
            TextBoxSaturdayInTabPageAvailability, TextBoxSundayInTabPageAvailability, TextBoxMondayInTabPageAvailability, TextBoxTuesdayInTabPageAvailability};

            foreach (var textbox in textBoxes)
            {
                ValidateAndConvertHourInShortFormatToLongIfNecesary(textbox);
            }
        }
        
        async Task FillComboBoxSelectEmployeeInAvailabilityTabPageAsync()
        {
            string position = CurrentSelectedPositionInComboBox();
            var employeeNames = await GetAllEmployeeNamesByPositionAsync(position);
            ComboBoxSelectEmployeeInTabPageAvailability.DataSource = employeeNames;
        }
        
        #endregion

        #endregion

        #region TabPage EmployeePetitions
        
        #region Methods

        async Task DeleteSelectedPetitionsOfAnEmployeeAsync()
        {
            var checkedPetitions = ListViewEmployeePetitionsInTabPagePetitions.CheckedIndices;
            var petition = new Petition() { EmployeeName = ComboBoxSelectEmployeeInTabPagePetition.Text };

            for (int i = 0; i < checkedPetitions.Count; i++)
            {
                var date = ListViewEmployeePetitionsInTabPagePetitions.Items[checkedPetitions[i]].Text;
                petition.Date = DateFunctions.FromDateTimeStringToLocalDate(date);

                _petitionRepo.Petition = petition;
                await _petitionRepo.DeleteAsync();
                ListViewEmployeePetitionsInTabPagePetitions.Items.RemoveAt(checkedPetitions[i]);
            }
        }

        async Task SaveEmployeePetitionAsync()
        {
            var petition = new Petition();
            petition.EmployeeName = ComboBoxSelectEmployeeInTabPagePetition.Text;
            petition.Date = DateFunctions.FromDateTimeToLocalDate(PetitionDatePicker.Value.Date);

            if (!IsFreeDayCheckBox.Checked)
            {
                if (string.IsNullOrEmpty(TextBoxPetitionAvailabilityHours.Text))
                {
                    TextBoxPetitionAvailabilityHours.Focus();
                    throw new Exception("No ha indicado la hora disponible que el empleado puede laborar.");
                }

                ValidateAndConvertHourInShortFormatToLongIfNecesary(TextBoxPetitionAvailabilityHours);

                var availability = TextBoxPetitionAvailabilityHours.Text.Replace(" ", "").Split('-');
                petition.AvailableFrom = TimeFunctions.ParseLocalTimeFromString(availability[0]);
                petition.AvailableTo = TimeFunctions.ParseLocalTimeFromString(availability[1]);
            }

            _petitionRepo.Petition = petition;
            await _petitionRepo.SaveAsync();
        }

        async Task FillComboBoxSelectEmployeeInPetitionsTabPageAsync()
        {
            string position = CurrentSelectedPositionInComboBox();
            var employeeNames = await GetAllEmployeeNamesByPositionAsync(position);
            ComboBoxSelectEmployeeInTabPagePetition.DataSource = employeeNames;
        }

        async Task ShowAllPetitionsOfAnEmployeeAsync() 
        {
            var employeePetitions = await _petitionRepo.GetEmployeePetitionsAsync(ComboBoxSelectEmployeeInTabPagePetition.Text);
            ListViewEmployeePetitionsInTabPagePetitions.Items.Clear();

            for (int i = 0; i < employeePetitions.Count; i++)
            {
                string availableFrom = employeePetitions[i].AvailableFrom.ToString("h:mmtt", _culture),
                       availableTo = employeePetitions[i].AvailableTo.ToString("h:mmtt", _culture),
                       availableFromTo = availableFrom == availableTo ? "Pidió día libre" : string.Format("{0} - {1}", availableFrom, availableTo);

                var availability = new ListViewItem(new string[] 
                { 
                    employeePetitions[i].Date.ToString(), 
                    employeePetitions[i].FreeDay ? "Cierto" : "Falso", 
                    availableFromTo
                });

                ListViewEmployeePetitionsInTabPagePetitions.Items.Add(availability);
            }
        }

        #endregion

        #region Events

        private async void ButtonDeletePetitionInTabPagePetition_Click(object sender, EventArgs e)
        {
            try
            {
                await DeleteSelectedPetitionsOfAnEmployeeAsync();
                await ShowMessageInLabelMessageOfFormMain("Las peticiones han sido eliminadas", "");
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        private void IsFreeDayCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            LabelPetitionExampleIndicator.Visible = !IsFreeDayCheckBox.Checked;
            TextBoxPetitionAvailabilityHours.Visible = !IsFreeDayCheckBox.Checked;
        }

        private async void ButtonSavePetitionInTabPagePetition_Click(object sender, EventArgs e)
        {
            try
            {
                await SaveEmployeePetitionAsync();
                await ShowMessageInLabelMessageOfFormMain("La petición para el " + PetitionDatePicker.Value.ToShortDateString() + " de " + ComboBoxSelectEmployeeInTabPagePetition .Text+ " se guardó.", "");
                await ShowAllPetitionsOfAnEmployeeAsync();
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        private async void ComboBoxSelectEmployeeInTabPagePetition_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                await ShowAllPetitionsOfAnEmployeeAsync();
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        #endregion

        #endregion

        #region TabPage EmployeeSchedule

        #region Methods

        async Task FillComboBoxSelectEmployeeInScheduleTabPageAsync()
        {
            string position = CurrentSelectedPositionInComboBox();
            var employeeNames = await GetAllEmployeeNamesByPositionAsync(position);
            ComboBoxSelectEmployeeInTabPageSchedule.DataSource = employeeNames;
        }
        #endregion

        #region Events

        

        #endregion

        #endregion
    }
}
