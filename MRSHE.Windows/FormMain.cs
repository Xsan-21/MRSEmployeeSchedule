using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core.Entities;
using MRSES.Core.Persistence;
using MRSES.Windows.Forms;
using MRSES.Core.Shared;
using MRSES.ExternalServices.CloudPersistence;
using NodaTime;

namespace MRSES.Windows
{
    public partial class FormMain : Form
    {
        #region Variables

        IBusinessRepository _businessRepo;
        IEmployeeRepository _employeeRepo;
        IAvailabilityRepository _availabilityRepo;
        IPetitionRepository _petitionRepo;
        IScheduleRepository _scheduleRepo;
        FormConfigureBusiness _formConfiguration;
        FormGenerateSchedule _formGenerateSchedule;
   
        #endregion

        #region Initialization

        public FormMain()
        {
            InitializeComponent();
            Width = 850; Height = 500;
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            FillWeekSelectorComboBox();
            PetitionDatePicker.MinDate = DateTime.Now.AddDays(1);
            TextBoxLocationInTabPageEmployeeInformation.Text = Configuration.Location;

            var tasks = new[] {
                FillPositionComboBoxAsync(),
                CompleteCustomSourceOfTextBoxPositionInTabPageEmployeeInformationAsync(),
                CompleteCustomSourceOfTextBoxLocationInTabPageEmployeeInformationAsync(),
                CompleteCustomSourceOfTextBoxDepartmentInTabPageEmployeeInformationAsync()
            };

            await Task.WhenAll(tasks);
        } 

        #endregion

        #region Shared Methods For Main Form

        async Task FillPositionComboBoxAsync()
        {
            using (_employeeRepo = new EmployeeRepository())
            {
                var positions = await _employeeRepo.GetPositionsAsync();
                ComboBoxPositionSelectorInFormMain.DataSource = positions; 
            }
        }

        void FillWeekSelectorComboBox()
        {
            var weeks = DateFunctions.GetPreviousAndNextThreeWeeks();
            ComboBoxWeekSelectorInFormMain.DataSource = weeks.ToArray();
            ComboBoxWeekSelectorInFormMain.SelectedIndex = 1; // here we select the current week
        }

        public async Task ShowMessageInLabelMessageOfFormMain(string message, string result, int delay)
        {
            LabelMessageInFormMain.Text = message;
            
            if(result == "error")
                LabelMessageInFormMain.BackColor = Color.Red;
            

            await Task.Delay(delay);
            LabelMessageInFormMain.BackColor = Color.DarkKhaki;
            LabelMessageInFormMain.Text = "Mensajes";
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
            return await _employeeRepo.GetEmployeeNamesByPositionAsync(position);
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

        private void opcionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_formConfiguration == null)
                _formConfiguration = new FormConfigureBusiness();

            _formConfiguration.ShowDialog();
        }

        async void ComboBoxPositionSelectorInFormMainValueChanged(object sender, EventArgs e)
        {
            try
            {
              await RunFunctionsWhenComboBoxPositionSelectorIndexChanges();
            }
            catch (Exception ex)
            {
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
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
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
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
                UpdateListViewScheduleByPositionAndWeekAsync(),
                PutEmployeeScheduleInTextBoxesAsync()                
            };

            await Task.WhenAll(tasks);
        }

        async Task RunFunctionsWhenComboBoxPositionSelectorIndexChanges()
        {
            var tasks = new Task[] 
            { 
                PutEmployeeScheduleInTextBoxesAsync(),
                UpdateListViewScheduleByDayAsync(),
                UpdateListViewScheduleByPositionAndWeekAsync(),
                FillListBoxEmployeesInEmployeeTabPageAsync(),
                FillComboBoxSelectEmployeeInScheduleTabPageAsync(),
                FillComboBoxSelectEmployeeInPetitionsTabPageAsync(),
                FillComboBoxSelectEmployeeInAvailabilityTabPageAsync()
            };

            await Task.WhenAll(tasks);
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
            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox(), Configuration.CultureInfo);
            ComboBoxSelectWeekDayForTurnsOfADay.DataSource = weekDays;
        }

        void PutDaysOfWeekInEmployeeScheduleListViewInTabPageViewSchedule()
        {
            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox(), Configuration.CultureInfo);
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

        async Task UpdateListViewScheduleByPositionAndWeekAsync()
        {
            ListViewEmployeeScheduleOfWeek.Visible = false;

            if (string.IsNullOrEmpty(CurrentSelectedPositionInComboBox()) || string.IsNullOrEmpty(ComboBoxWeekSelectorInFormMain.Text))
                return;

            var position = CurrentSelectedPositionInComboBox();
            var week = CurrentSelectedWeekInComboBox();

            using (_scheduleRepo = new ScheduleRepository())
            {
                var scheduleByPosition = await _scheduleRepo.GetScheduleByPositionAsync(position, week);

                InsertEmployeeScheduleInListView(scheduleByPosition); 
            }
        }

        void AjustListViewScheduleOfWeekWidth()
        {
            for (int i = 0; i <= ListViewEmployeeScheduleOfWeek.Columns.Count - 1; i++)
                ListViewEmployeeScheduleOfWeek.Columns[i].Width = -2;
        }

        void InsertEmployeeScheduleInListView(Schedule[] schedule)
        {
            ListViewEmployeeScheduleOfWeek.Items.Clear();

            foreach (var employee in schedule)
            {
                var employeeSchedule = new ListViewItem(new string[]
                    {
                        employee.Employee,
                        employee.WeekDays[0].ToString(),
                        employee.WeekDays[1].ToString(),
                        employee.WeekDays[2].ToString(),
                        employee.WeekDays[3].ToString(),
                        employee.WeekDays[4].ToString(),
                        employee.WeekDays[5].ToString(),
                        employee.WeekDays[6].ToString(),
                        employee.HoursOfWeek.ToString(),
                        employee.AmountOfTurns.ToString()
                    });

                ListViewEmployeeScheduleOfWeek.Items.Add(employeeSchedule);
            }

            AjustListViewScheduleOfWeekWidth();
            ListViewEmployeeScheduleOfWeek.Visible = true;
        }

        async Task UpdateListViewScheduleByDayAsync()
        {
            ListViewScheduleByDay.Visible = false;

            if (string.IsNullOrEmpty(CurrentSelectedPositionInComboBox()) || string.IsNullOrEmpty(ComboBoxWeekSelectorInFormMain.Text))
                return;

            var dayIndex = ComboBoxSelectWeekDayForTurnsOfADay.SelectedIndex;
            var date = CurrentSelectedWeekInComboBox().PlusDays(dayIndex);

            using (_scheduleRepo = new ScheduleRepository())
            {
                var scheduleOfDay = await _scheduleRepo.ShowTurnsByDay(CurrentSelectedPositionInComboBox(), CurrentSelectedWeekInComboBox(), date);
            
                if (scheduleOfDay.Values.All(t => t.Hours == 0))
                {
                    await ShowMessageInLabelMessageOfFormMain("No existen turnos para este día", "", 3000);
                    return;
                }

                InsertScheduleByDayInListView(scheduleOfDay); 
            }
        }

        void AjustListViewScheduleByDayWidth()
        {
            for (int i = 0; i <= ListViewScheduleByDay.Columns.Count - 1; i++)
                ListViewScheduleByDay.Columns[i].Width = -2;
        }

        void InsertScheduleByDayInListView(Dictionary<string,Turn> scheduleDay)
        {
            ListViewScheduleByDay.Items.Clear();

            foreach (var turn in scheduleDay)
            {
                var daySchedule = new ListViewItem(new string[]
                {
                    turn.Key,
                    turn.Value.FirstTurn,
                    turn.Value.SecondTurn,
                    turn.Value.Hours.ToString()
                });

                ListViewScheduleByDay.Items.Add(daySchedule);
            }

            AjustListViewScheduleByDayWidth();
            ListViewScheduleByDay.Visible = true;
        }

        #endregion

        #region Events

        private async void ComboBoxSelectWeekDayForTurnsOfADay_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                await UpdateListViewScheduleByDayAsync();
            }
            catch (Exception ex)
            {                
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
            }
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

        async void ButtonSaveInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            try
            {
                DisableButtonsInTabPageEmployeeInformation();
                await SaveEmployeeInformationAsync();
                ClearEmployeeInformationTextBoxes();
                await FillPositionComboBoxAsync();
            }
            catch(Exception ex) 
            {
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
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
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
            }
            finally
            {
                EnableButtonsInTabPageEmployeeInformation();
            }
        }

        private void ButtonClearTextBoxesInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            ClearEmployeeInformationTextBoxes();
        }

        private void CheckBoxEnableTextBoxStoreInTabPageEmployeeInformation_CheckedChanged(object sender, EventArgs e)
        {
            var _checked = CheckBoxEnableTextBoxStoreInTabPageEmployeeInformation.Checked;
            if (_checked)
                TextBoxLocationInTabPageEmployeeInformation.ReadOnly = false;
            else
            {
                TextBoxLocationInTabPageEmployeeInformation.ReadOnly = true;
                TextBoxLocationInTabPageEmployeeInformation.Text = Configuration.Location;
            }
        }
        
        #endregion

        #region Methods

        async Task CompleteCustomSourceOfTextBoxPositionInTabPageEmployeeInformationAsync()
        {
            using (_employeeRepo = new EmployeeRepository())
            {
                var positions = await _employeeRepo.GetPositionsAsync();
                TextBoxEmployeePositionInTabPageEmployeeInformation.AutoCompleteCustomSource.AddRange(positions.ToArray()); 
            }
        }

        async Task CompleteCustomSourceOfTextBoxLocationInTabPageEmployeeInformationAsync()
        {
            using (_businessRepo = new BusinessRepository())
            {
                var _locations = await _businessRepo.GetBusinessLocationsAsync(Configuration.Business);
                var _locationNames = _locations.Select(l => l.City).ToArray();
                TextBoxLocationInTabPageEmployeeInformation.AutoCompleteCustomSource.AddRange(_locationNames); 
            }
        }

        async Task CompleteCustomSourceOfTextBoxDepartmentInTabPageEmployeeInformationAsync()
        {
            using (_businessRepo = new BusinessRepository())
            {
                var dapartments = await _businessRepo.GetDepartmentsAsync();
                TextBoxEmployeeDepartmentInTabPageEmployeeInformation.AutoCompleteCustomSource.AddRange(dapartments); 
            }
        }

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

        bool ValidateEmployeeInformation()
        {
            string position = TextBoxEmployeePositionInTabPageEmployeeInformation.Text;

            bool result =
                TextBoxEmployeeIDInTabPageEmployeeInformation.Text == TextBoxEmployeeIDInTabPageEmployeeInformation.Tag.ToString() ? false
                : position == TextBoxEmployeePositionInTabPageEmployeeInformation.Tag.ToString() ? false
                : TextBoxEmployeeNameInTabPageEmployeeInformation.Text == TextBoxEmployeeNameInTabPageEmployeeInformation.Tag.ToString() ? false
                : StringFunctions.StringIsNullOrEmpty(TextBoxEmployeeIDInTabPageEmployeeInformation.Text) ? false
                : StringFunctions.StringIsNullOrEmpty(TextBoxEmployeePositionInTabPageEmployeeInformation.Text) ? false
                : StringFunctions.StringIsNullOrEmpty(TextBoxEmployeeNameInTabPageEmployeeInformation.Text) ? false
                : true;

            if (!result)
                return result;

            var store = TextBoxLocationInTabPageEmployeeInformation.Text;

            if (StringFunctions.StringIsNullOrEmpty(store))
                return false;
            else if (!TextBoxLocationInTabPageEmployeeInformation.AutoCompleteCustomSource.Contains(store))
            {
                TextBoxLocationInTabPageEmployeeInformation.Focus();
                    throw new Exception("No existe la tienda especificada para la transferencia.");
            }

            TextBoxEmployeePositionInTabPageEmployeeInformation.Text = char.ToUpper(position[0]) + position.Substring(1);

            return result;
        }

        async Task DeleteEmployeeAfterUserConfirmationAsync()
        {
            if (ListBoxEmployeesInEmployeeTabPage.SelectedIndex < 0)
            {
                await ShowMessageInLabelMessageOfFormMain("Seleccione un empleado de la lista para eliminarlo.", "error", 5000);
                return;
            }

            var dialogResult = AlertUser.Message("¿Desea borrar el empleado " + TextBoxEmployeeNameInTabPageEmployeeInformation.Text + " del sistema?", "Eliminar empleado", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
            {
                await DeleteEmployeeInformationInTabEmployeeAsync();
                await ShowMessageInLabelMessageOfFormMain("El empleado " + TextBoxEmployeeNameInTabPageEmployeeInformation.Text + " se eliminó.", "done", 3000);
                ClearEmployeeInformationTextBoxes();
                await FillListBoxEmployeesInEmployeeTabPageAsync();
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
            CheckBoxEnableTextBoxStoreInTabPageEmployeeInformation.Checked = false;
        }

        async Task SaveEmployeeInformationAsync()
        {
            bool result = ValidateEmployeeInformation();

            if (result)
            {
                await SaveEmployeeInformationInTabEmployeeAsync();
                await ShowMessageInLabelMessageOfFormMain("Nuevo empleado agregado o actualizado.", "", 3000);
                await FillListBoxEmployeesInEmployeeTabPageAsync();
            }
            else
            {
                await ShowMessageInLabelMessageOfFormMain("No ha completado la información requerida del empleado.", "error", 5000);     
            }
        }

        async Task SaveEmployeeInformationInTabEmployeeAsync()
        {
            var employee = new Employee
            {
                Name = TextBoxEmployeeNameInTabPageEmployeeInformation.Text,
                Id = TextBoxEmployeeIDInTabPageEmployeeInformation.Text,
                Department = TextBoxEmployeeDepartmentInTabPageEmployeeInformation.Text,
                IsStudent = CheckBoxIsStudentInTabPageEmployeeInformation.Checked,
                JobType = CheckBoxIsPartTimeInTabPageEmployeeInformation.Checked == true ? "Part-Time" : "Full-Time",
                PhoneNumber = TextBoxEmployeePhoneNumberInTabPageEmployeeInformation.Text,
                Location = TextBoxLocationInTabPageEmployeeInformation.Text,
                Position = TextBoxEmployeePositionInTabPageEmployeeInformation.Text
            };

            using (_businessRepo = new BusinessRepository())
            {
                using (_employeeRepo = new EmployeeRepository(employee))
                {
                    var _location_obj_id = await _businessRepo.GetLocationObjectIdAsync(employee.Location);
                    employee.Location = _location_obj_id;
                    employee.ObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(employee.Name); // If exists, else a new objectID will be assigned later.
                    await _employeeRepo.SaveAsync();
                }
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
            var employeeObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(employeeName);
            var employeeInfo = await _employeeRepo.GetEmployeeAsync(employeeObjectId);

            TextBoxEmployeeNameInTabPageEmployeeInformation.Text = employeeInfo.Name;
            TextBoxEmployeeDepartmentInTabPageEmployeeInformation.Text = employeeInfo.Department;
            TextBoxEmployeeIDInTabPageEmployeeInformation.Text = employeeInfo.Id;
            TextBoxEmployeePhoneNumberInTabPageEmployeeInformation.Text = employeeInfo.PhoneNumber;
            TextBoxEmployeePositionInTabPageEmployeeInformation.Text = employeeInfo.Position;
            CheckBoxIsStudentInTabPageEmployeeInformation.Checked = employeeInfo.IsStudent;
            CheckBoxIsPartTimeInTabPageEmployeeInformation.Checked = employeeInfo.JobType == "Part-Time" ? true : false;
            CheckBoxIsFulltimeInTabPageEmployeeInformation.Checked = employeeInfo.JobType == "Full-Time" ? true : false;
        }

        async Task DeleteEmployeeInformationInTabEmployeeAsync()
        {
            using (_employeeRepo = new EmployeeRepository())
            {
                var employee = await _employeeRepo.GetEmployeeAsync(TextBoxEmployeeIDInTabPageEmployeeInformation.Text);
                _employeeRepo.Employee = employee;
                await _employeeRepo.DeleteAsync();
            }
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
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
            }
        }

        private async void ButtonSaveInTabPageAvailability_Click(object sender, EventArgs e)
        {
            try
            {
                await SaveEmployeeAvailabilityAsync();
                await ShowMessageInLabelMessageOfFormMain("Se guardo la disponibilidad de " + ComboBoxSelectEmployeeInTabPageAvailability.Text, "", 3000);
            }
            catch (Exception ex)
            {
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
            }
        }

        #endregion

        #region Methods

        async Task<Availability> GetEmployeeAvailabilityFromTextBoxesAsync()
        {
            string
                _employeeName = ComboBoxSelectEmployeeInTabPageAvailability.Text,
                _employeeObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(_employeeName), 
                _wednesday = TextBoxWednesdayInTabPageAvailability.Text.Trim(),
                _thursday = TextBoxThursdayInTabPageAvailability.Text.Trim(),
                _friday = TextBoxFridayInTabPageAvailability.Text.Trim(),
                _saturday = TextBoxSaturdayInTabPageAvailability.Text.Trim(),
                _sunday = TextBoxSundayInTabPageAvailability.Text.Trim(),
                _monday = TextBoxMondayInTabPageAvailability.Text.Trim(),
                _tuesday = TextBoxTuesdayInTabPageAvailability.Text.Trim();

            var _availability = new Availability();
            _availability.Employee = _employeeObjectId;
            _availability.Wednesday = PossibleNotAvailableInputs(_wednesday);
            _availability.Thursday = PossibleNotAvailableInputs(_thursday);
            _availability.Friday = PossibleNotAvailableInputs(_friday);
            _availability.Saturday = PossibleNotAvailableInputs(_saturday);
            _availability.Sunday = PossibleNotAvailableInputs(_sunday);
            _availability.Monday = PossibleNotAvailableInputs(_monday);
            _availability.Tuesday = PossibleNotAvailableInputs(_tuesday);

            return _availability;
        }

        string PossibleNotAvailableInputs(string time)
        {
            var _possibleNotAvailableInputs = new[] { "nd", "no disponible" };
            return time.Contains('-') ? time : _possibleNotAvailableInputs.Any(p => p == time) ? "not available" : null;
        }

        async Task SaveEmployeeAvailabilityAsync()
        {
            ValidateAvailabilityHourFormatOfTextBoxes();

            using(_availabilityRepo = new AvailabilityRepository())
            {
                var availability = await GetEmployeeAvailabilityFromTextBoxesAsync();
                _availabilityRepo.Availability = availability;
                await _availabilityRepo.SaveAsync();
            }
        }

        async Task SendEmployeeAvailabilityToTextBoxesAsync()
        {
            using (_availabilityRepo = new AvailabilityRepository())
            {
                var employeeObjectId = await EmployeeRepository.GetEmployeeObjectIdAsync(ComboBoxSelectEmployeeInTabPageAvailability.Text);
                var availability = await _availabilityRepo.GetAvailabilityAsync(employeeObjectId);
                TextBoxWednesdayInTabPageAvailability.Text = TranslateAvailabilityText(availability.Wednesday);
                TextBoxThursdayInTabPageAvailability.Text = TranslateAvailabilityText(availability.Thursday);
                TextBoxFridayInTabPageAvailability.Text =  TranslateAvailabilityText(availability.Friday);
                TextBoxSaturdayInTabPageAvailability.Text = TranslateAvailabilityText(availability.Saturday);
                TextBoxSundayInTabPageAvailability.Text = TranslateAvailabilityText(availability.Sunday);
                TextBoxMondayInTabPageAvailability.Text = TranslateAvailabilityText(availability.Monday);
                TextBoxTuesdayInTabPageAvailability.Text = TranslateAvailabilityText(availability.Tuesday);
            }
        }

        string TranslateAvailabilityText(string text)
        {
            return text == "available" ? "disponible"
                                        : text == "not available" ? "no disponible"
                                        : text; 
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

            using (_petitionRepo = new PetitionRepository())
            {
                while (countOfSelectedPetitions-- > 0)
                {
                    var firstPetitionSelected = ListViewEmployeePetitionsInTabPagePetitions.CheckedItems[0];

                    _petitionRepo.Petition = new Petition()
                    {
                        Employee = ComboBoxSelectEmployeeInTabPagePetition.Text,
                        Date = DateFunctions.FromDateTimeStringToLocalDate(firstPetitionSelected.Text)
                    };

                    await _petitionRepo.DeleteAsync();
                    ListViewEmployeePetitionsInTabPagePetitions.Items.RemoveAt(firstPetitionSelected.Index);
                } 
            }
        }

        async Task SaveEmployeePetitionAsync()
        {
            using (_petitionRepo = new PetitionRepository())
            {
                var petition = new Petition();
                petition.Employee = await EmployeeRepository.GetEmployeeObjectIdAsync(ComboBoxSelectEmployeeInTabPagePetition.Text);
                petition.Date = DateFunctions.FromDateTimeToLocalDate(PetitionDatePicker.Value.Date);
                
                if (!IsFreeDayCheckBox.Checked)
                {
                    var availability = TimeFunctions.GetTurnInAndOut(TextBoxPetitionAvailabilityHours.Text);
                    petition.AvailableFrom = availability[0];
                    petition.AvailableTo = availability[1];
                }

                _petitionRepo.Petition = petition;
                await _petitionRepo.SaveAsync(); 
            }
        }

        async Task FillComboBoxSelectEmployeeInPetitionsTabPageAsync()
        {
            string position = CurrentSelectedPositionInComboBox();
            var employeeNames = await GetAllEmployeeNamesByPositionAsync(position);
            ComboBoxSelectEmployeeInTabPagePetition.DataSource = employeeNames;
        }

        async Task ShowAllPetitionsOfAnEmployeeAsync()
        {
            var employeePetitions = new List<Petition>();

            using (_petitionRepo = new PetitionRepository())
            {
                employeePetitions = await _petitionRepo.GetEmployeePetitionsAsync(ComboBoxSelectEmployeeInTabPagePetition.Text); 
            }

            ListViewEmployeePetitionsInTabPagePetitions.Items.Clear();

            for (int i = 0; i < employeePetitions.Count; i++)
            {
                string availableFrom = employeePetitions[i].AvailableFrom.ToString("h:mmtt", Configuration.CultureInfo),
                       availableTo = employeePetitions[i].AvailableTo.ToString("h:mmtt", Configuration.CultureInfo),
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
                await ShowMessageInLabelMessageOfFormMain("Las peticiones han sido eliminadas", "", 3000);
            }
            catch (Exception ex)
            {
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
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
                await ShowMessageInLabelMessageOfFormMain("La petición para el " + PetitionDatePicker.Value.ToShortDateString() + " de " + ComboBoxSelectEmployeeInTabPagePetition.Text + " se guardó.", "", 3000);
            }
            catch (Exception ex)
            {
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
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
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
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

            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox(), Configuration.CultureInfo);

            for (int i = 0; i < 7; i++)
            {
                labelDays[i].Text = weekDays[i];
            }
        }

        void ShowTotalHoursInCorrespondingLabelDay(string firstTurn, string secondTurn, Label totalHoursInDay)
        {
            totalHoursInDay.Text = TimeFunctions.TotalTurnHours(firstTurn, secondTurn).ToString();
            SumTotalWeekHours();
        }

        void SumTotalTurnHours(string textBoxTag)
        {
            switch (textBoxTag)
            {
                case "day1FirstTurn":
                case "day1SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay1FirstTurnInTabPageSchedule.Text, TextBoxDay1SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay1);
                    break;
                case "day2FirstTurn":
                case "day2SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay2FirstTurnInTabPageSchedule.Text, TextBoxDay2SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay2);
                    break;
                case "day3FirstTurn":
                case "day3SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay3FirstTurnInTabPageSchedule.Text, TextBoxDay3SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay3);
                    break;
                case "day4FirstTurn":
                case "day4SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay4FirstTurnInTabPageSchedule.Text, TextBoxDay4SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay4);
                    break;
                case "day5FirstTurn":
                case "day5SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay5FirstTurnInTabPageSchedule.Text, TextBoxDay5SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay5);
                    break;
                case "day6FirstTurn":
                case "day6SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay6FirstTurnInTabPageSchedule.Text, TextBoxDay6SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay6);
                    break;
                case "day7FirstTurn":
                case "day7SecondTurn":
                    ShowTotalHoursInCorrespondingLabelDay(TextBoxDay7FirstTurnInTabPageSchedule.Text, TextBoxDay7SecondTurnInTabPageSchedule.Text, LabelTotalHoursDay7);
                    break;
                default:
                    break;
            }
        }

        void SumTotalWeekHours()
        {
            double totalHoursInWeek = 0;

            totalHoursInWeek += double.Parse(LabelTotalHoursDay1.Text);
            totalHoursInWeek += double.Parse(LabelTotalHoursDay2.Text);
            totalHoursInWeek += double.Parse(LabelTotalHoursDay3.Text);
            totalHoursInWeek += double.Parse(LabelTotalHoursDay4.Text);
            totalHoursInWeek += double.Parse(LabelTotalHoursDay5.Text);
            totalHoursInWeek += double.Parse(LabelTotalHoursDay6.Text);
            totalHoursInWeek += double.Parse(LabelTotalHoursDay7.Text);

            LabelTotalHoursInWeek.Text = totalHoursInWeek.ToString();
        }

        Schedule GetEmployeeWeekSchedule()
        {
            var week = CurrentSelectedWeekInComboBox();
            var employeeName = ComboBoxSelectEmployeeInTabPageSchedule.Text;
            var employeeSchedule = new Schedule(employeeName, week);

            var firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay1FirstTurnInTabPageSchedule.Text);
            var secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay1SecondTurnInTabPageSchedule.Text);

            employeeSchedule.WeekDays[0].TurnHours = new[] {firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay2FirstTurnInTabPageSchedule.Text);
            secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay2SecondTurnInTabPageSchedule.Text);

            employeeSchedule.WeekDays[1].TurnHours = new[] { firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay3FirstTurnInTabPageSchedule.Text);
            secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay3SecondTurnInTabPageSchedule.Text);

            employeeSchedule.WeekDays[2].TurnHours = new[] { firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay4FirstTurnInTabPageSchedule.Text);
            secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay4SecondTurnInTabPageSchedule.Text);

            employeeSchedule.WeekDays[3].TurnHours = new[] { firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay5FirstTurnInTabPageSchedule.Text);
            secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay5SecondTurnInTabPageSchedule.Text);

            employeeSchedule.WeekDays[4].TurnHours = new[] { firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay6FirstTurnInTabPageSchedule.Text);
            secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay6SecondTurnInTabPageSchedule.Text);
            employeeSchedule.WeekDays[5].TurnHours = new[] { firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            firstTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay7FirstTurnInTabPageSchedule.Text);
            secondTurn = TimeFunctions.GetTurnInAndOut(TextBoxDay7SecondTurnInTabPageSchedule.Text);

            employeeSchedule.WeekDays[6].TurnHours = new[] { firstTurn[0], firstTurn[1], secondTurn[0], secondTurn[1] };

            return employeeSchedule;
        }

        /// <summary>
        /// Call VerifyIfTheEmployeeCanDoTheTurnAsync() instead
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeTurns"></param>
        /// <returns>False if any turn conflicts with employee petition or availability</returns>
        async Task<bool> VerifyWithAvailabilityAndPetitionsAsync(string employeeName, Turn[] employeeTurns)
        {
            bool result = true;

            for (int i = 0; i < employeeTurns.Length; i++)
            {
                if (employeeTurns[i].IsFreeDay)
                    continue;

                var checkAvailability = await CheckEmployeeAvailability(employeeName, employeeTurns[i]);

                if (!checkAvailability)
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

        async Task<bool> CheckEmployeeAvailability(string employeeName, Turn turn)
        {
            using (_availabilityRepo = new AvailabilityRepository())
            {
                using (_petitionRepo = new PetitionRepository())
                {
                    var checkAvailability = await _availabilityRepo.CanDoTheTurnAsync(employeeName, turn);
                    var checkPetition = await _petitionRepo.CanDoTheTurnAsync(employeeName, turn);

                    return checkAvailability && checkPetition ? true : false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns>False if any turn conflicts with work's cycle of 24 hours</returns>
        async Task<bool> VerifyScheduleWith24CycleHourAsync(ISchedule schedule)
        {
            using (_scheduleRepo = new ScheduleRepository())
            {
                foreach (var turn in schedule.WeekDays)
                {
                    bool result = await _scheduleRepo.ViolateCycleOfTwentyFourHour(schedule.Employee, turn);

                    if (result)
                    {
                        await MarkTurnThatViolate24HourCycleAsync(turn.Date.IsoDayOfWeek);
                        return false;
                    }
                }

                return true; 
            }
        }

        async Task ShowMessageOf24HourCycleViolation(TextBox textBox, string dayOfWeek)
        {
            textBox.Focus();
            await ShowMessageInLabelMessageOfFormMain("El dia " + dayOfWeek +  " conflige con el ciclo de 24 horas.", "error", 5000);
        }

        async Task MarkTurnThatViolate24HourCycleAsync(NodaTime.IsoDayOfWeek day)
        {
            switch (day) // TODO the position of week days varies depending on business's first day of week. Requires Fix
            {
                case NodaTime.IsoDayOfWeek.Friday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay3FirstTurnInTabPageSchedule, "viernes");
                    break;
                case NodaTime.IsoDayOfWeek.Monday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay6FirstTurnInTabPageSchedule, "lunes");
                    break;
                case NodaTime.IsoDayOfWeek.Saturday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay4FirstTurnInTabPageSchedule, "sábado");
                    break;
                case NodaTime.IsoDayOfWeek.Sunday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay5FirstTurnInTabPageSchedule, "domingo");
                    break;
                case NodaTime.IsoDayOfWeek.Thursday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay2FirstTurnInTabPageSchedule, "jueves");
                    break;
                case NodaTime.IsoDayOfWeek.Tuesday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay7FirstTurnInTabPageSchedule, "martes");
                    break;
                case NodaTime.IsoDayOfWeek.Wednesday:
                    await ShowMessageOf24HourCycleViolation(TextBoxDay1FirstTurnInTabPageSchedule, "miércoles");
                    break;
                default:
                    break;
            }
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
            var tasks = new[]
            {
                VerifyScheduleWith24CycleHourAsync(employeeSchedule),
                VerifyWithAvailabilityAndPetitionsAsync(employeeSchedule.Employee, employeeSchedule.WeekDays)
            };

            var result = await Task.WhenAll(tasks);

            if (result.All(a => a == true))
            {
                using (_scheduleRepo = new ScheduleRepository())
                {
                    _scheduleRepo.Schedule.Add(employeeSchedule);
                    await _scheduleRepo.SaveAsync();
                    return true;
                }
            }

            return false;
        }

        async Task PutEmployeeScheduleInTextBoxesAsync()
        {
            if (StringFunctions.StringIsNullOrEmpty(ComboBoxWeekSelectorInFormMain.Text) || StringFunctions.StringIsNullOrEmpty(ComboBoxSelectEmployeeInTabPageSchedule.Text))
                return;

            using (_scheduleRepo = new ScheduleRepository())
            {
                var empSchedule = await _scheduleRepo.GetEmployeeScheduleAsync(ComboBoxSelectEmployeeInTabPageSchedule.Text, CurrentSelectedWeekInComboBox());

                TextBoxDay1FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[0].FirstTurn;
                TextBoxDay1SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[0].SecondTurn;

                TextBoxDay2FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[1].FirstTurn;
                TextBoxDay2SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[1].SecondTurn;

                TextBoxDay3FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[2].FirstTurn;
                TextBoxDay3SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[2].SecondTurn;

                TextBoxDay4FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[3].FirstTurn;
                TextBoxDay4SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[3].SecondTurn;

                TextBoxDay5FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[4].FirstTurn;
                TextBoxDay5SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[4].SecondTurn;

                TextBoxDay6FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[5].FirstTurn;
                TextBoxDay6SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[5].SecondTurn;

                TextBoxDay7FirstTurnInTabPageSchedule.Text = empSchedule.WeekDays[6].FirstTurn;
                TextBoxDay7SecondTurnInTabPageSchedule.Text = empSchedule.WeekDays[6].SecondTurn; 
            }
        }

        /// <summary>
        /// Code left for next iteration
        /// </summary>
        //async Task SyncWithCloudDataAsync()
        //{
        //    await Task.Run(async () => {
        //       await  new ExternalServices.DataSynchronization().SyncDataAsync();
        //    });
        //}

        #endregion

        #region Events

        void InputCharacterIsValidOnKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !StringFunctions.InputIsValidHourCharacter(e.KeyChar);
        }

        void ConvertShortHourFormatToLongWhenTextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            bool shortHourValidFormat = ChangeShortHourFormatToLongIfPossible(textBox);

            // workaround for blank space left in textbox TODO implement better code for handling deletion of hour in textboxes.
            bool isEmptyTurn = textBox.TextLength == 1 && textBox.Text == " ";
            if (isEmptyTurn) textBox.Clear();

            ButtonSaveInTabPageSchedule.Enabled = isEmptyTurn || shortHourValidFormat;
            SumTotalTurnHours(textBox.Tag.ToString());
        }

        private void ButtonAutomaticSchedule_Click(object sender, EventArgs e)
        {
            _formGenerateSchedule = new FormGenerateSchedule();
            _formGenerateSchedule.Show();
        }

        async void ButtonSaveInTabPageSchedule_Click(object sender, EventArgs e)
        {
            try
            {
                await TrySaveScheduleAsync();
            }
            catch (Exception ex)
            {
                DisableTurnTextBoxes(false);
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
            }
            finally
            {
                DisableTurnTextBoxes(false);
            }
        }

        async Task TrySaveScheduleAsync()
        {
            ButtonSaveInTabPageSchedule.Enabled = false;
            DisableTurnTextBoxes(true);

            LabelMessageInFormMain.Text = "Verificando horario...";
            await Task.Delay(2000);

            var result = await SaveEmployeeScheduleAsync(GetEmployeeWeekSchedule());

            if (result)
            {
                await ShowMessageInLabelMessageOfFormMain("Horario guardado!", "", 3000);
            }
            else
            {
                throw new Exception("El horario no se guardó");
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
                await ShowMessageInLabelMessageOfFormMain(ex.Message, "error", 5000);
            }
        }

        private void ToolStripMenuItemPrintSchedule_Click(object sender, EventArgs e)
        {
            new FormPrintSchedule().Show();
        }
        #endregion

        #endregion

        /// <summary>
        /// Code left for next iteration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSyncAllData_Click(object sender, EventArgs e)
        {
            //ButtonSyncAllData.Enabled = false;
            //LabelMessageInFormMain.Text = "Sincronizando datos...";
            //await Task.Delay(2000);
            //await SyncWithCloudDataAsync();
            //await ShowMessageInLabelMessageOfFormMain("Sincronización completada", "", 3000);
            //ButtonSyncAllData.Enabled = true;
        }
    }
}
