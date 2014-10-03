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
                default:
                    AlertUser.Message("Coming soon!");
                    break;
            }
        }

        #endregion

        #region Mouse hover 
        
        #endregion

        #region Textbox text changes

        void SetDefaultTextIndicatorIfTextBoxIsEmpty(object sender, EventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox.Text == "" || textbox.Text == "\b")
            {
                textbox.Text = textbox.Tag.ToString();
                textbox.ForeColor = Color.Gray;
            }                               
        }

        #endregion           

        #region Textbox click

        void SetCursorToTheStartOfTextBox(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == textBox.Tag.ToString())
                textBox.SelectionStart = 0;
        }

        #endregion       

        #region Textbox key press      

        void RemoveDefaultTextIndicatorFromTextBox(object sender, KeyPressEventArgs e)
        {
            var textbox = sender as TextBox;


            if (textbox.Text == textbox.Tag.ToString())
                textbox.Text = e.KeyChar.ToString();
            else
                textbox.ForeColor = Color.Black;

            // Workaround
            //============================================================================================
            // for any reason when the user press a key the character is written two times in the text box.
            // This code removes the first duplicated character.
            if (textbox.TextLength == 2)
            {
                // do to the problem this is always true, the above condition is used 
                // for remove the duplicated character in the beginning of text.
                if (textbox.Text[0] == textbox.Text[1])
                {
                    textbox.Text = textbox.Text.Remove(0, 1);
                    textbox.SelectionStart = 1;
                }
            }                                                                                             
        }

        #endregion

        #endregion
    }
}
