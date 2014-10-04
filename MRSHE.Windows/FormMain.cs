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

namespace MRSES.Windows
{
    public partial class FormMain : Form
    {
        #region Variables
        #endregion

        #region Properties

        string CurrentActiveTextBoxDefaultText { get; set; }

        #endregion

        #region Initialization

        public FormMain()
        {
            InitializeComponent();
            this.Width = 850; this.Height = 500; 
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        } 

        #endregion

        #region Methods

        public void ShowMessageInLabelMessageOfFormMain(string message)
        {
            LabelMessageInFormMain.Text = message;
        }

        #endregion        

        #region Events

        #region Button clicks

        private void ButtonSyncSchedule_Click(object sender, EventArgs e)
        {
            
        }

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

        #region Textbox text changes

        void SetDefaultTextIndicatorIfTextBoxIsEmpty(object sender, EventArgs e)
        {
            StringFunctions.SetDefaultTextIndicatorIfTextBoxIsEmpty(sender as TextBox, e);                               
        }

        #endregion           

        #region Textbox click

        void SetCursorToTheStartOfTextBox(object sender, EventArgs e)
        {
            StringFunctions.SetCursorToTheStartOfTextBox(sender as TextBox, e);
        }

        #endregion       

        #region Textbox key press      

        void RemoveDefaultTextIndicatorFromTextBox(object sender, KeyPressEventArgs e)
        {
            StringFunctions.RemoveDefaultTextIndicatorFromTextBox(sender as TextBox, e);                                                                                             
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
    }
}
