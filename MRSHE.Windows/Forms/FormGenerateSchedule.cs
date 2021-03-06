﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core.Persistence;
using MRSES.Core.Shared;

namespace MRSES.Windows.Forms
{
    public partial class FormGenerateSchedule : Form
    {
        IEmployeeRepository _employeeRepo;
              
        public FormGenerateSchedule()
        {
            InitializeComponent();
            FillWeekSelectorComboBox();
        }       

        private async void FormGenerateSchedule_Load(object sender, EventArgs e)
        {
            await FillPositionComboBoxAsync();
        }

        void InputCharacterIsValidOnKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !StringFunctions.InputIsValidHourCharacter(e.KeyChar);
        }

        void InputCharacterIsNumber(object sender, KeyPressEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "^[0-9\\b]+$"))
                e.Handled = false;
            else
                e.Handled = true;
        }

        void ConvertShortHourFormatToLongWhenTextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            bool result = ChangeShortHourFormatToLongIfPossible(textBox);
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

        NodaTime.LocalDate CurrentSelectedWeekInComboBox()
        {
            var string_week = ComboBoxWeekSelectorInFormGenerateSchedule.Text;
            return DateFunctions.FromLocalDateStringToLocalDate(string_week);
        }

        void FillWeekSelectorComboBox()
        {
            var weeks = DateFunctions.GetPreviousAndNextThreeWeeks();
            ComboBoxWeekSelectorInFormGenerateSchedule.DataSource = weeks.ToArray();
            ComboBoxWeekSelectorInFormGenerateSchedule.SelectedIndex = 1; // here we select the current week
        }

        async Task FillPositionComboBoxAsync()
        {
            using (_employeeRepo = new EmployeeRepository())
            {
                var positions = await _employeeRepo.GetPositionsAsync();
                ComboBoxPositionSelectorInFormGenerateSchedule.DataSource = positions; 
            }
        }

        private void ComboBoxWeekSelectorInFormGenerateSchedule_SelectedValueChanged(object sender, EventArgs e)
        {
            
            Label[] labelDays = { LabelFirstDay, LabelSecondDay, LabelThirdDay,
                                LabelFourthDay, LabelFifthDay, LabelSixthDay,
                                LabelSeventhDay};

            var weekDays = DateFunctions.DaysOfWeekInString(CurrentSelectedWeekInComboBox(), Configuration.CultureInfo);

            for (int i = 0; i < 7; i++)
            {
                labelDays[i].Text = weekDays[i];
            }
        }
    }
}
