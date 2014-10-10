using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MRSES.Core.Shared;
//using MRSES.ExternalServices.Parse;

namespace MRSES.Windows.Forms
{
    public partial class FormPetitions : Form
    {
        //PetitionRepository _petitionRepository = new PetitionRepository(); // TODO usar Postgres
        //EmployeeRepository _employeeRepository = new EmployeeRepository(); // TODO usar Postgres

        public FormPetitions()
        {
            InitializeComponent();
        }

        private void FormPetitions_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;            
        }

        async void FormPetitions_Load(object sender, EventArgs e)
        {
            FillWeekComboBox();
            await FillPositionComboBoxAsync();
        }

        private async Task ShowPetitionsAsync()
        {
            RichTextBoxPetitionViewInFormPetitions.Clear();
            LabelMessageResult.Text = "Buscando peticiones...";

            string date = ComboBoxWeekSelectorInFormPetition.Text;
            string position = ComboBoxPositionSelectorInFormPetition.Text;

            NodaTime.LocalDate week = DateFunctions.FromLocalDateStringToLocalDate(date);

            //var petitions = await _petitionRepository.GetAllPetitions(week, position); // TODO usar Postgres
            //var preparePetitions = await PreparePetitionsAsync(petitions);
            //RichTextBoxPetitionViewInFormPetitions.Text = preparePetitions.ToString();
            LabelMessageResult.Visible = false;
            if (RichTextBoxPetitionViewInFormPetitions.Text == "")
            {
                LabelMessageResult.Text = "No hay peticiones para esta semana";
                LabelMessageResult.Visible = true;
            }                
        }

        async Task<StringBuilder> PreparePetitionsAsync(MRSES.Core.Entities.Petition[] petitions)
        {
            return await Task.Run(() => 
            {
                StringBuilder sb = new StringBuilder();

                var query = from n in petitions
                            where n.Date >= MRSES.Core.Entities.WorkWeek.CurrentWeek()
                            where n.Date <= MRSES.Core.Entities.WorkWeek.CurrentWeek().PlusWeeks(1)
                            group new
                            {
                                Name = n.Employee.Name,
                                Availability = NodaTime.Period.Between(n.AvailableFrom, n.AvailableTo).Hours == 0
                                                ? "No disponible"
                                                : string.Format("Disponible de {0} a {1}", n.AvailableFrom.TimeOfDay, n.AvailableTo.TimeOfDay)
                            }
                            by n.Date;

                foreach (var item in query)
                {
                    sb.Append(item.Key).AppendLine().AppendLine();

                    foreach (var item2 in item)
                    {
                        sb.Append("\t");
                        sb.Append(item2.Name);
                        sb.Append(", ");
                        sb.Append(item2.Availability);
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                }

                return sb;
            });            
        }

        private void FillWeekComboBox()
        {
            if (ComboBoxWeekSelectorInFormPetition.Items.Count <= 1)
                foreach (var week in ComboBoxFunctions.GetCurrentAndNextThreeWeeks())                
                    ComboBoxWeekSelectorInFormPetition.Items.Add(week);                       
        }

        async Task FillPositionComboBoxAsync()
        {
            //if (ComboBoxPositionSelectorInFormPetition.Items.Count < 1)
            //    foreach (var position in await _employeeRepository.GetPositions()) // TODO usar Postgres
            //        ComboBoxPositionSelectorInFormPetition.Items.Add(position);       
        }

        async void SelectedValueChangedInComboBoxWeekOrPositionAsync(object sender, EventArgs e)
        {
            if (ComboBoxWeekSelectorInFormPetition.Text != "Seleccione semana" && ComboBoxPositionSelectorInFormPetition.Text != "Seleccione posición")
                await ShowPetitionsAsync();
        }
    }
}
