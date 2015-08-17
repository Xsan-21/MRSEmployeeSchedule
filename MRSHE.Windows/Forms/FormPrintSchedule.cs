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
using MRSES.Core.Shared;
using MRSES.Core.Entities;

namespace MRSES.Windows.Forms
{
    public partial class FormPrintSchedule : Form
    {
        IEmployeeRepository _employeeRepo;
        IScheduleRepository _scheduleRepo;

        NodaTime.LocalDate SelectedWeek
        {
            get
            {
                var week = ComboBoxWeekSelectorInFormPrintSchedule.Text;
                return DateFunctions.FromLocalDateStringToLocalDate(week);
            }
        }

        string Position{get{return ComboBoxPositionSelectorInFormPrintSchedule.Text;}}

        public FormPrintSchedule()
        {
            InitializeComponent();
        }

        private async void FormPrintSchedule_Load(object sender, EventArgs e)
        {
            FillWeekSelectorComboBox();
            await FillPositionComboBoxAsync();
        }

        void FillWeekSelectorComboBox()
        {
            var weeks = DateFunctions.GetCurrentAndNextThreeWeeks();
            ComboBoxWeekSelectorInFormPrintSchedule.DataSource = weeks.ToArray();
        }

        async Task FillPositionComboBoxAsync()
        {
            using (_employeeRepo = new EmployeeRepository())
            {
                var positions = await _employeeRepo.GetPositionsAsync();
                ComboBoxPositionSelectorInFormPrintSchedule.DataSource = positions; 
            }
        }

        void ComboBoxesInFormPrintScheduleSelectedValueChanged(object sender, EventArgs e)
        {
            var week = SelectedWeek.ToString("yyyy-MM-dd", Configuration.CultureInfo);
            TextBoxReportNameInFormPrintSchedule.Clear();
            TextBoxReportNameInFormPrintSchedule.Text = string.Format("Horario-{0}-Semana-{1}", Position, week);
        }

        private async void ButtonSaveInTabPageEmployeeInformation_Click(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    await PrintScheduleAsync();
                    break;
                }
                catch (Exception ex)
                {
                    var alert = AlertUser.Message(ex.Message, "Error al intento de escribir el reporte", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    if (alert == DialogResult.Abort)
                        break;
                }
            }
        }

        async Task PrintScheduleAsync()
        {
            var scheduleToPrint = await GetScheduleAsync();

            if (ScheduleWeekHaveNoTurns(scheduleToPrint))
                return;

            using (var _printer = new PrintSchedule())
            {
                _printer.Position = ComboBoxPositionSelectorInFormPrintSchedule.Text;
                _printer.ReportName = TextBoxReportNameInFormPrintSchedule.Text;
                _printer.Week = SelectedWeek;
                _printer.Schedule = scheduleToPrint;
                _printer.Print();

                ShowSuccessfulReportMessageToUser(_printer.ReportFolderLocation);
            }
        }

        void ShowSuccessfulReportMessageToUser(string fileName)
        {
            var message = AlertUser.Message("Se creó el reporte satisfactoriamente. ¿Desea abrirlo?", "Reporte terminado", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            if (message == DialogResult.Yes)
                System.Diagnostics.Process.Start("explorer.exe", fileName);
        }

        bool ScheduleWeekHaveNoTurns(Schedule[] sch)
        {
            if (sch.All(a => a.AmountOfTurns == 0))
            {
                AlertUser.Message("No existe horario para la semana seleccionada.", "No hay horario", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return true;
            }

            return false;
        }

        async Task<Schedule[]> GetScheduleAsync()
        {
            using (_scheduleRepo = new ScheduleRepository())
            {
                return await _scheduleRepo.GetScheduleByPositionAsync(Position, SelectedWeek); 
            }
        }
    }
}
