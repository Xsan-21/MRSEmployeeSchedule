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
        EmployeeRepository employeeRepository;
        AvailabilityRepository _availabilityRepo;
        PetitionRepository _petitionRepo;
        ITurnRepository _turnRepo;
   
        #endregion

        #region Properties

        #endregion

        #region Initialization

        public FormMain()
        {
            InitializeComponent();
            this.Width = 850; this.Height = 500;
            PetitionDatePicker.MinDate = DateTime.Now.AddDays(1);
            FillWeekSelectorComboBox();

            employeeRepository = new EmployeeRepository();
            _availabilityRepo = new AvailabilityRepository();
            _petitionRepo = new PetitionRepository();
            _turnRepo = new TurnRepository();
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await FillPositionComboBoxAsync();
        } 

        #endregion

        #region Shared Methods For Main Form

        NodaTime.LocalTime[] GetTurnInAndOut(string turn)
        {
            if(string.IsNullOrEmpty(turn))
                return new NodaTime.LocalTime[]{new NodaTime.LocalTime(), new NodaTime.LocalTime()};

            return TimeFunctions.GetTurnInAndOut(turn);
        }

        async Task FillPositionComboBoxAsync()
        {
            var positions = await employeeRepository.GetPositionsAsync();
            ComboBoxPositionSelectorInFormMain.DataSource = positions;
        }

        void FillWeekSelectorComboBox()
        {
            var weeks = DateFunctions.GetPreviousAndNextThreeWeeks();
            ComboBoxWeekSelectorInFormMain.DataSource = weeks.ToArray();
            ComboBoxWeekSelectorInFormMain.SelectedIndex = 1; // here we select the current week
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

        NodaTime.LocalDate CurrentSelectedWeekInComboBox()
        {
            var string_week = ComboBoxWeekSelectorInFormMain.Text;
            return DateFunctions.FromLocalDateStringToLocalDate(string_week);
        }

        async Task<List<string>> GetAllEmployeeNamesByPositionAsync(string position)
        {
            return await employeeRepository.GetEmployeeNamesByPositionAsync(position);
        }

        bool ChangeShortHourFormatToLongIfPossible(TextBox textBox)
        {
            bool result = false;

            if (TimeFunctions.TryChangeShortHourFormatToLongFormat(textBox))
            {
                if (TimeFunctions.FormatOfTurnIsValid(textBox.Text))
                {
                    result = true;
                }
            }

            return result;
        }

        #endregion        

        #region MainForm Events

        async void ComboBoxPositionSelectorInFormMainValueChanged(object sender, EventArgs e)
        {
            try
            {
                await RunFunctionsWhenComboBoxPositionSelectorIndexChanges();
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        private async void ComboBoxWeekSelectorInFormMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                await RunFunctionsWhenComboBoxWeekSelectorIndexChanges();
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        #region FunctionsToRunWhenComboBoxWeekSelectorOrPostionIndexChanges

        async Task RunFunctionsWhenComboBoxWeekSelectorIndexChanges()
        {
            PutDaysOfWeekInComboBoxSelectWeekDayForTurnsOfADayInTabPageViewSchedule();
            PutDayOfWeekInTheLabelsInScheduleTabPage();
            PutDaysOfWeekInEmployeeScheduleListViewInTabPageViewSchedule();

            var tasks = new Task[] 
            { 
                PutEmployeeScheduleInTextBoxesAsync(),
                InsertEmployeeScheduleByPositionAndWeekAsync()
            };

            for (int i = 0; i < tasks.Length; i++)
            {
                await tasks[i];
            }
        }

        async Task RunFunctionsWhenComboBoxPositionSelectorIndexChanges()
        {
            var tasks = new Task[] 
            { 
                PutEmployeeScheduleInTextBoxesAsync(),
                InsertEmployeeScheduleByPositionAndWeekAsync(),
                FillListBoxEmployeesInEmployeeTabPageAsync(),
                FillComboBoxSelectEmployeeInScheduleTabPageAsync(),
                FillComboBoxSelectEmployeeInPetitionsTabPageAsync(),
                FillComboBoxSelectEmployeeInAvailabilityTabPageAsync()
            };

            for (int i = 0; i < tasks.Length; i++)
            {
                await tasks[i];
            }
        }

        #endregion

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
        
        #endregion     

        #endregion

        #region ViewWeekTabPageInMainForm

        #region Methods

        void PutDaysOfWeekInComboBoxSelectWeekDayForTurnsOfADayInTabPageViewSchedule()
        {
            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox());
            ComboBoxSelectWeekDayForTurnsOfADay.DataSource = weekDays;
        }

        void PutDaysOfWeekInEmployeeScheduleListViewInTabPageViewSchedule()
        {
            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox());
            ColumnEmployeeName.Text = "Empleado";
            ColumnDay1.Text = weekDays[0];
            ColumnDay2.Text = weekDays[1];
            ColumnDay3.Text = weekDays[2];
            ColumnDay4.Text = weekDays[3];
            ColumnDay5.Text = weekDays[4];
            ColumnDay6.Text = weekDays[5];
            ColumnDay7.Text = weekDays[6];
            ColumnHours.Text = "Horas";
            ColumnAmountOfTurns.Text = "Turnos";
        }

        async Task InsertEmployeeScheduleByPositionAndWeekAsync()
        {
            ListViewEmployeeScheduleOfWeek.Visible = false;
            if (string.IsNullOrEmpty(CurrentSelectedPositionInComboBox()) || string.IsNullOrEmpty(ComboBoxWeekSelectorInFormMain.Text))
                return;
            
            var position = CurrentSelectedPositionInComboBox();
            var week = CurrentSelectedWeekInComboBox();

            var scheduleByPosition = await _turnRepo.GetScheduleByPositionAsync(position, week);

            
            ListViewEmployeeScheduleOfWeek.Items.Clear();

            foreach (var employee in scheduleByPosition)
            {
                var employeeSchedule = new ListViewItem(new string[] 
                    { 
                        employee.Name,
                        employee.Turns[0].ToString(),
                        employee.Turns[1].ToString(),
                        employee.Turns[2].ToString(),
                        employee.Turns[3].ToString(),
                        employee.Turns[4].ToString(),
                        employee.Turns[5].ToString(),
                        employee.Turns[6].ToString(),
                        employee.HoursOfWeek.ToString(),
                        employee.AmountOfTurns.ToString()
                    });

                ListViewEmployeeScheduleOfWeek.Items.Add(employeeSchedule); 
            }                      

            AjustEmployeeScheduleOfWeekWidth();
            ListViewEmployeeScheduleOfWeek.Visible = true;
        }

        void AjustEmployeeScheduleOfWeekWidth()
        {
            for (int i = 0; i <= 7; i++)
                ListViewEmployeeScheduleOfWeek.Columns[i].Width = -2;
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
                ClearEmployeeInformationTextBoxes();
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
            _idOfCurrentSelectedEmployeeInEmployeeTabPage = string.Empty;
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
                OldID = _idOfCurrentSelectedEmployeeInEmployeeTabPage
            };

            employeeRepository.Employee = employee;
            await employeeRepository.SaveAsync();
            _idOfCurrentSelectedEmployeeInEmployeeTabPage = string.Empty;
        }

        async Task DeleteEmployeeInformationInTabEmployeeAsync()
        {
            employeeRepository.Employee = new Employee() { Name = TextBoxEmployeeNameInTabPageEmployeeInformation.Text,  ID = TextBoxEmployeeIDInTabPageEmployeeInformation.Text };
            await employeeRepository.DeleteAsync();
            await FillPositionComboBoxAsync();
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

        Availability 
            GetEmployeeAvailabilityFromTextBoxes()
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
                Wednesday = _wednesday.Contains('-') ? _wednesday : _possibleNotAvailableInputs.Any(p => p == _wednesday) ? "not available" : null,
                Thursday = _thursday.Contains('-') ? _thursday : _possibleNotAvailableInputs.Any(p => p == _thursday) ? "not available" : null,
                Friday = _friday.Contains('-') ? _friday : _possibleNotAvailableInputs.Any(p => p == _friday) ? "not available" : null,
                Saturday = _saturday.Contains('-') ? _saturday : _possibleNotAvailableInputs.Any(p => p == _saturday) ? "not available" : null,
                Sunday = _sunday.Contains('-') ? _sunday : _possibleNotAvailableInputs.Any(p => p == _sunday) ? "not available" : null,
                Monday = _monday.Contains('-') ? _monday : _possibleNotAvailableInputs.Any(p => p == _monday) ? "not available" : null,
                Tuesday = _tuesday.Contains('-') ? _tuesday : _possibleNotAvailableInputs.Any(p => p == _tuesday) ? "not available" : null
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

            TextBoxWednesdayInTabPageAvailability.Text = availability.Wednesday == "available" ? "disponible" : availability.Wednesday == "not available" ? "no disponible" : availability.Wednesday;
            TextBoxThursdayInTabPageAvailability.Text = availability.Thursday == "available" ? "disponible" : availability.Thursday == "not available" ? "no disponible" : availability.Thursday;
            TextBoxFridayInTabPageAvailability.Text = availability.Friday == "available" ? "disponible" : availability.Friday == "not available" ? "no disponible" : availability.Friday;
            TextBoxSaturdayInTabPageAvailability.Text = availability.Saturday == "available" ? "disponible" : availability.Saturday == "not available" ? "no disponible" : availability.Saturday;
            TextBoxSundayInTabPageAvailability.Text = availability.Sunday == "available" ? "disponible" : availability.Sunday == "not available" ? "no disponible" : availability.Sunday;
            TextBoxMondayInTabPageAvailability.Text = availability.Monday == "available" ? "disponible" : availability.Monday == "not available" ? "no disponible" : availability.Monday;
            TextBoxTuesdayInTabPageAvailability.Text = availability.Tuesday == "available" ? "disponible" : availability.Tuesday == "not available" ? "no disponible" :  availability.Tuesday;
        }

        void ValidateAvailabilityHourFormatOfTextBoxes()
        {
            var textBoxes = new[] { TextBoxWednesdayInTabPageAvailability, TextBoxThursdayInTabPageAvailability, TextBoxFridayInTabPageAvailability,
            TextBoxSaturdayInTabPageAvailability, TextBoxSundayInTabPageAvailability, TextBoxMondayInTabPageAvailability, TextBoxTuesdayInTabPageAvailability};

            foreach (var textbox in textBoxes)
                TimeFunctions.TryChangeShortHourFormatToLongFormat(textbox);
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
            int countOfSelectedPetitions = ListViewEmployeePetitionsInTabPagePetitions.CheckedItems.Count;
            
            while (countOfSelectedPetitions-- > 0)
            {
                var firstPetitionSelected = ListViewEmployeePetitionsInTabPagePetitions.CheckedItems[0];

                _petitionRepo.Petition = new Petition()
                {
                    EmployeeName = ComboBoxSelectEmployeeInTabPagePetition.Text,
                    Date = DateFunctions.FromDateTimeStringToLocalDate(firstPetitionSelected.Text)
                };

                await _petitionRepo.DeleteAsync();
                ListViewEmployeePetitionsInTabPagePetitions.Items.RemoveAt(firstPetitionSelected.Index);
            }
        }

        async Task SaveEmployeePetitionAsync()
        {
            var petition = new Petition();
            petition.EmployeeName = ComboBoxSelectEmployeeInTabPagePetition.Text;
            petition.Date = DateFunctions.FromDateTimeToLocalDate(PetitionDatePicker.Value.Date);

            if (!IsFreeDayCheckBox.Checked)
            {
                var availability = GetTurnInAndOut(TextBoxPetitionAvailabilityHours.Text);
                petition.AvailableFrom = availability[0];
                petition.AvailableTo = availability[1];
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
                string availableFrom = employeePetitions[i].AvailableFrom.ToString("h:mmtt", Core.Configuration.CultureInfo),
                       availableTo = employeePetitions[i].AvailableTo.ToString("h:mmtt", Core.Configuration.CultureInfo),
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

        void EnableOrDisableButtonDeletePetitionInTabPagePetition()
        {
            var itemsChecked = ListViewEmployeePetitionsInTabPagePetitions.CheckedItems.Count;

            ButtonDeletePetitionInTabPagePetition.Enabled = itemsChecked > 0 ? true : false;
        }

        void ResetPetitionSelectionFields()
        {
            PetitionDatePicker.Value = PetitionDatePicker.MinDate;
            IsFreeDayCheckBox.Checked = false;
            LabelPetitionExampleIndicator.Visible = true;
            TextBoxPetitionAvailabilityHours.Text = TextBoxPetitionAvailabilityHours.Tag.ToString();
            TextBoxPetitionAvailabilityHours.Visible = true;
            ButtonSavePetitionInTabPagePetition.Enabled = false;
        }

        #endregion

        #region Events

        private void TextBoxPetitionAvailabilityHours_TextChanged(object sender, EventArgs e)
        {
            StringFunctions.ChangeTextColor(sender as TextBox);
            ButtonSavePetitionInTabPagePetition.Enabled = ChangeShortHourFormatToLongIfPossible(TextBoxPetitionAvailabilityHours);
        }

        private async void ButtonDeletePetitionInTabPagePetition_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonDeletePetitionInTabPagePetition.Enabled = false;
                await DeleteSelectedPetitionsOfAnEmployeeAsync();
                await ShowMessageInLabelMessageOfFormMain("Las peticiones han sido eliminadas", "");
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
            finally
            {
                EnableOrDisableButtonDeletePetitionInTabPagePetition();
            }
        }

        private void IsFreeDayCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            LabelPetitionExampleIndicator.Visible = !IsFreeDayCheckBox.Checked;
            TextBoxPetitionAvailabilityHours.Visible = !IsFreeDayCheckBox.Checked;
            ButtonSavePetitionInTabPagePetition.Enabled = IsFreeDayCheckBox.Checked;
        }

        private async void ButtonSavePetitionInTabPagePetition_Click(object sender, EventArgs e)
        {
            try
            {
                await SaveEmployeePetitionAsync();
                await ShowAllPetitionsOfAnEmployeeAsync();
                ResetPetitionSelectionFields();
                await ShowMessageInLabelMessageOfFormMain("La petición para el " + PetitionDatePicker.Value.ToShortDateString() + " de " + ComboBoxSelectEmployeeInTabPagePetition .Text+ " se guardó.", "");
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

        private void ListViewEmployeePetitionsInTabPagePetitions_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            EnableOrDisableButtonDeletePetitionInTabPagePetition();
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

        

        void PutDayOfWeekInTheLabelsInScheduleTabPage()
        {
            Label[] labelDays = { LabelFirstDayOfWeekInTabPageSchedule, LabelSecondDayOfWeekInTabPageSchedule, LabelThirdDayOfWeekInTabPageSchedule,
                                LabelFourthDayOfWeekInTabPageSchedule, LabelFifthDayOfWeekInTabPageSchedule, LabelSixthDayOfWeekInTabPageSchedule,
                                LabelSeventhDayOfWeekInTabPageSchedule};

            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox());

            for (int i = 0; i < 7; i++)
            {
                labelDays[i].Text = weekDays[i];
            }
        }

        void SumTotalTurnHours(string textBoxTag)
        {
            string firstTurn, secondTurn, totalHours, result;
            switch (textBoxTag)
            {
                case "day1FirstTurn":
                case "day1SecondTurn":
                    firstTurn = TextBoxDay1FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay1SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInFirstDayInTabPageSchedule.Text = result;
                    break;
                case "day2FirstTurn":
                case "day2SecondTurn":
                    firstTurn = TextBoxDay2FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay2SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInSecondDayInTabPageSchedule.Text = result;
                    break;
                case "day3FirstTurn":
                case "day3SecondTurn":
                    firstTurn = TextBoxDay3FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay3SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInThirdDayInTabPageSchedule.Text = result;
                    break;
                case "day4FirstTurn":
                case "day4SecondTurn":
                    firstTurn = TextBoxDay4FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay4SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInFourthDayInTabPageSchedule.Text = result;
                    break;
                case "day5FirstTurn":
                case "day5SecondTurn":
                    firstTurn = TextBoxDay5FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay5SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInFifthDayInTabPageSchedule.Text = result;
                    break;
                case "day6FirstTurn":
                case "day6SecondTurn":
                    firstTurn = TextBoxDay6FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay6SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInSixthDayInTabPageSchedule.Text = result;
                    break;
                case "day7FirstTurn":
                case "day7SecondTurn":
                    firstTurn = TextBoxDay7FirstTurnInTabPageSchedule.Text;
                    secondTurn = TextBoxDay7SecondTurnInTabPageSchedule.Text;
                    totalHours = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
                    result = totalHours == "0" ? "" : totalHours == "1" ? "1 hora" : totalHours + " horas";
                    LabelTotalHoursInSeventhDayInTabPageSchedule.Text = result;
                    break;
                default:
                    break;
            }
        }

        Schedule GetEmployeeWeekSchedule()
        {
            var week = CurrentSelectedWeekInComboBox();
            var employeeName = ComboBoxSelectEmployeeInTabPageSchedule.Text;
            var employeeSchedule = new Schedule(week, employeeName);

            var firstTurn = GetTurnInAndOut(TextBoxDay1FirstTurnInTabPageSchedule.Text);
            var secondTurn = GetTurnInAndOut(TextBoxDay1SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[0] = Turn.Create(week, firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            firstTurn = GetTurnInAndOut(TextBoxDay2FirstTurnInTabPageSchedule.Text);
            secondTurn = GetTurnInAndOut(TextBoxDay2SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[1] = Turn.Create(week.PlusDays(1), firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            firstTurn = GetTurnInAndOut(TextBoxDay3FirstTurnInTabPageSchedule.Text);
            secondTurn = GetTurnInAndOut(TextBoxDay3SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[2] = Turn.Create(week.PlusDays(2), firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            firstTurn = GetTurnInAndOut(TextBoxDay4FirstTurnInTabPageSchedule.Text);
            secondTurn = GetTurnInAndOut(TextBoxDay4SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[3] = Turn.Create(week.PlusDays(3), firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            firstTurn = GetTurnInAndOut(TextBoxDay5FirstTurnInTabPageSchedule.Text);
            secondTurn = GetTurnInAndOut(TextBoxDay5SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[4] = Turn.Create(week.PlusDays(4), firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            firstTurn = GetTurnInAndOut(TextBoxDay6FirstTurnInTabPageSchedule.Text);
            secondTurn = GetTurnInAndOut(TextBoxDay6SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[5] = Turn.Create(week.PlusDays(5), firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            firstTurn = GetTurnInAndOut(TextBoxDay7FirstTurnInTabPageSchedule.Text);
            secondTurn = GetTurnInAndOut(TextBoxDay7SecondTurnInTabPageSchedule.Text);
            employeeSchedule.Turns[6] = Turn.Create(week.PlusDays(6), firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1]);

            return employeeSchedule;
        }

        /// <summary>
        /// Call VerifyIfTheEmployeeCanDoTheTurnAsync() instead
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeTurns"></param>
        /// <returns>False if any turn conflicts with employee petition or availability</returns>
        async Task<bool> VerifyWithAvailabilityAndPetitions(string employeeName, List<Turn> employeeTurns)
        {
            bool result = true;

            for (int i = 0; i < employeeTurns.Count; i++)
            {
                if (employeeTurns[i].IsFreeDay)
                    continue;

                var checkAvailability = await _availabilityRepo.CanDoTheTurnAsync(employeeName, employeeTurns[i]);
                var checkPetition = await _petitionRepo.CanDoTheTurnAsync(employeeName, employeeTurns[i]);

                if (!checkAvailability || !checkPetition)
                {
                    ShowLabelNotAvailableForTheTurn(i, true);
                    result = false;
                }
                else
                {
                    ShowLabelNotAvailableForTheTurn(i, false);
                    ButtonSaveInTabPageSchedule.Enabled = false; // Enabling the button again is handled by SelectedTextChanged event.
                }
            }

            return result;
        }

        void DisableTurnTextBoxes(bool on)
        {
            TextBoxDay1FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay1SecondTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay2FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay2SecondTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay3FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay3SecondTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay4FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay4SecondTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay5FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay5SecondTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay6FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay6SecondTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay7FirstTurnInTabPageSchedule.Enabled = !on;
            TextBoxDay7SecondTurnInTabPageSchedule.Enabled = !on;
        }

        void ShowLabelNotAvailableForTheTurn(int index, bool visible)
        {
            switch (index)
            {
                case 0:
                    LabelNotAvailableDay1.Visible = visible;
                    break;
                case 1:
                    LabelNotAvailableDay2.Visible = visible;
                    break;
                case 2:
                    LabelNotAvailableDay3.Visible = visible;
                    break;
                case 3:
                    LabelNotAvailableDay4.Visible = visible;
                    break;
                case 4:
                    LabelNotAvailableDay5.Visible = visible;
                    break;
                case 5:
                    LabelNotAvailableDay6.Visible = visible;
                    break;
                case 6:
                    LabelNotAvailableDay7.Visible = visible;
                    break;
                default:
                    break;
            }
        }

        async Task<bool> SaveEmployeeScheduleAsync(Schedule employeeSchedule)
        {
            var canDoTheTurn = await VerifyWithAvailabilityAndPetitions(employeeSchedule.Name, employeeSchedule.Turns);

            if (canDoTheTurn)
            {
                _turnRepo.Schedule = employeeSchedule;
                await _turnRepo.SaveAsync();
                return true;
            }

            return false;
        }

        async Task PutEmployeeScheduleInTextBoxesAsync()
        {
            if (string.IsNullOrEmpty(ComboBoxWeekSelectorInFormMain.Text) || string.IsNullOrEmpty(ComboBoxSelectEmployeeInTabPageSchedule.Text))
                return;

            var empSchedule = await _turnRepo.GetEmployeeScheduleAsync(ComboBoxSelectEmployeeInTabPageSchedule.Text, CurrentSelectedWeekInComboBox());

            TextBoxDay1FirstTurnInTabPageSchedule.Text = empSchedule.Turns[0].FirstTurn;
            TextBoxDay1SecondTurnInTabPageSchedule.Text = empSchedule.Turns[0].SecondTurn;

            TextBoxDay2FirstTurnInTabPageSchedule.Text = empSchedule.Turns[1].FirstTurn;
            TextBoxDay2SecondTurnInTabPageSchedule.Text = empSchedule.Turns[1].SecondTurn;

            TextBoxDay3FirstTurnInTabPageSchedule.Text = empSchedule.Turns[2].FirstTurn;
            TextBoxDay3SecondTurnInTabPageSchedule.Text = empSchedule.Turns[2].SecondTurn;

            TextBoxDay4FirstTurnInTabPageSchedule.Text = empSchedule.Turns[3].FirstTurn;
            TextBoxDay4SecondTurnInTabPageSchedule.Text = empSchedule.Turns[3].SecondTurn;

            TextBoxDay5FirstTurnInTabPageSchedule.Text = empSchedule.Turns[4].FirstTurn;
            TextBoxDay5SecondTurnInTabPageSchedule.Text = empSchedule.Turns[4].SecondTurn;

            TextBoxDay6FirstTurnInTabPageSchedule.Text = empSchedule.Turns[5].FirstTurn;
            TextBoxDay6SecondTurnInTabPageSchedule.Text = empSchedule.Turns[5].SecondTurn;

            TextBoxDay7FirstTurnInTabPageSchedule.Text = empSchedule.Turns[6].FirstTurn;
            TextBoxDay7SecondTurnInTabPageSchedule.Text = empSchedule.Turns[6].SecondTurn;
        }

        #endregion

        #region Events

        void InputCharacterIsValidOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "^[.0-9apm:\\s\\-\\b]$"))
                e.Handled = false;
            else
                e.Handled = true;
        }

        void ConvertShortHourFormatToLongWhenTextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            bool result = ChangeShortHourFormatToLongIfPossible(textBox);
            ButtonSaveInTabPageSchedule.Enabled = result || string.IsNullOrEmpty(textBox.Text);
            SumTotalTurnHours(textBox.Tag.ToString());
        }

        async void ButtonSaveInTabPageSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonSaveInTabPageSchedule.Enabled = false;
                DisableTurnTextBoxes(true);
                LabelVerfyingScheduleMessage.Text = "Verificando horario...";
                var result = await SaveEmployeeScheduleAsync(GetEmployeeWeekSchedule());
                if (result)
                {
                    DisableTurnTextBoxes(false);
                    LabelVerfyingScheduleMessage.Text = "Horario guardado!";
                    await Task.Delay(5000);
                }
                else
                {
                    DisableTurnTextBoxes(false);
                    LabelVerfyingScheduleMessage.Text = "El horario no se guardó";
                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
            finally
            {
                LabelVerfyingScheduleMessage.Text = "";
            }
        }

        private async void ComboBoxSelectEmployeeInTabPageSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                await PutEmployeeScheduleInTextBoxesAsync();
            }
            catch (Exception ex)
            {
                new Task(async () => await ShowMessageInLabelMessageOfFormMain(ex.Message, "error")).RunSynchronously();
            }
        }

        #endregion     

        #endregion
    }
}
