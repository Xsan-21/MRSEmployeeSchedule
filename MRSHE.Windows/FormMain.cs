using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core.Entities;
using MRSES.Core.Shared;
using MRSES.Windows.Forms;
using MRSES.ExternalServices.Postgres;

namespace MRSES.Windows
{
    public partial class FormMain : Form
    {
        #region Variables

        FormPetitions formPetitions = new FormPetitions();
        EmployeeRepository employeeRepository = new EmployeeRepository();

        #endregion

        #region Properties

        #endregion

        #region Initialization

        public FormMain()
        {
            InitializeComponent();
            this.Width = 850; this.Height = 500; 
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await FillPositionComboBoxAsync();
            FillWeekSelectorComboBox();
        } 

        #endregion

        #region Methods

        async Task FillPositionComboBoxAsync()
        {
            var positions = await employeeRepository.GetPositionsAsync();
            ComboBoxPositionSelectorInFormMain.Items.AddRange(positions.ToArray());
        }

        void FillWeekSelectorComboBox()
        {
            foreach (var week in ComboBoxFunctions.GetCurrentAndNextThreeWeeks())
            {
                ComboBoxWeekSelectorInFormMain.Items.Add(week); 
            }
        }

        public void ShowMessageInLabelMessageOfFormMain(string message)
        {
            LabelMessageInFormMain.Text = message;
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
                ButtonSaveInTabPageEmployeeInformation.Enabled = false;
                await SaveEmployeeInformationInTabEmployeeAsync();
                LabelMessageInFormMain.Text = "Nuevo empleado agregado.";
            }
            catch (Exception ex)
            {
                LabelMessageInFormMain.Text = ex.Message;
            }
            finally
            {
                ButtonSaveInTabPageEmployeeInformation.Enabled = true;
            }            
        }

        private async void ButtonDeleteInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonDeleteInTabPageEmployeeInformation.Enabled = false;
                await DeleteEmployeeInformationInTabEmployeeAsync();                
            }
            catch (Exception ex)
            {
                LabelMessageInFormMain.Text = ex.Message;
            }
            finally
            {
                ClearEmployeeInformationTextBoxes();
                ButtonDeleteInTabPageEmployeeInformation.Enabled = true;
            }
        }
        
        #endregion

        #region Methods

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

        async Task FillListBoxEmployeesInEmployeeTabPageAsync()
        {
            string position = ComboBoxPositionSelectorInFormMain.Text;
            var employeeNames = await employeeRepository.GetEmployeeNamesByPosition(position);
            ListBoxEmployeesInEmployeeTabPage.Items.Clear();
            ListBoxEmployeesInEmployeeTabPage.Items.AddRange(employeeNames.ToArray());
        }

        async Task SendInformationToTextboxesAsync()
        {
            string employeeName = ListBoxEmployeesInEmployeeTabPage.SelectedItem.ToString();
            var employeeInfo = await employeeRepository.GetEmployeeAsync(employeeName);

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
                Store = ExternalServices.Configuration.StoreLocation
            };

            employeeRepository.Employee = employee;
            await employeeRepository.SaveAsync();
        }

        async Task DeleteEmployeeInformationInTabEmployeeAsync()
        {
            employeeRepository.Employee = new Employee() { ID = TextBoxEmployeeIDInTabPageEmployeeInformation.Text, Store = ExternalServices.Configuration.StoreLocation };
            await employeeRepository.DeleteAsync();
            ListBoxEmployeesInEmployeeTabPage.Items.Remove(ListBoxEmployeesInEmployeeTabPage.SelectedItem);
            LabelMessageInFormMain.Text = "El empleado " + TextBoxEmployeeNameInTabPageEmployeeInformation.Text + " se eliminó.";            
        }

        #endregion         

        #endregion        
    }
}
