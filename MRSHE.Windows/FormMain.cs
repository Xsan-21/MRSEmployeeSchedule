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

        FormPetitions formPetitions;
        EmployeeRepository employeeRepository;
   
        #endregion

        #region Properties

        #endregion

        #region Initialization

        public FormMain()
        {
            InitializeComponent();
            this.Width = 850; this.Height = 500;

            employeeRepository = new EmployeeRepository();
            formPetitions = new FormPetitions();
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

        #endregion        

        #region Events

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

        async void RemoveDefaultTextIndicatorInTextBoxAsync(object sender, EventArgs e)
        {
            await StringFunctions.RemoveDefaultTextIndicatorAsync(sender as TextBox);                                        
        }

        async void SetDefaultTextIndicatorInTextBoxAsync(object sender, EventArgs e)
        {
            await StringFunctions.SetDefaultTextIndicatorInTextBoxAsync(sender as TextBox);
        }

        async void ChangeColorOfTextAsync(object sender, EventArgs e)
        {
            await StringFunctions.ChangeTextColorAsync(sender as TextBox);
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

        async void ComboBoxPositionSelectorInFormMainValueChanged(object sender, EventArgs e)
        {
            await FillListBoxEmployeesInEmployeeTabPageAsync();
        }

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
                : string.IsNullOrEmpty(TextBoxEmployeeIDInTabPageEmployeeInformation.Text) ? false
                : string.IsNullOrEmpty(TextBoxEmployeePositionInTabPageEmployeeInformation.Text) ? false
                : string.IsNullOrEmpty(TextBoxEmployeeNameInTabPageEmployeeInformation.Text) ? false
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
            string position = ComboBoxPositionSelectorInFormMain.Text;
            var employeeNames = await employeeRepository.GetEmployeeNamesByPositionAsync(position);
            ListBoxEmployeesInEmployeeTabPage.Items.Clear();
            ListBoxEmployeesInEmployeeTabPage.Items.AddRange(employeeNames.ToArray());
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
    }
}
