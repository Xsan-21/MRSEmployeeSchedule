using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using MRSES.Core.Shared;
using MRSES.Windows.Properties;
using System.Net.Mail;

namespace MRSES.Windows.Forms
{
    public partial class FormFeedBack : Form
    {
        FeedBack _feedBack;

        public FormFeedBack()
        {
            InitializeComponent();
            TextBoxUserFeedbackInFormFeedBack.TextChanged += (sender, e) => 
            {
                //StringFunctions.ChangeTextColor(sender as TextBox);
                DisableButtonSendFeedBackIfTextBoxFeedBackHasInvalidText();
            };

            TextBoxUserFeedbackInFormFeedBack.Enter += (sender, e) =>
            {
                //StringFunctions.RemoveDefaultTextIndicator(sender as TextBox);                
            };

            TextBoxUserFeedbackInFormFeedBack.Leave += (sender, e) =>
            {
                //StringFunctions.SetDefaultTextIndicatorInTextBox(sender as TextBox);
            }; 
        }

        private async void ButtonSendFeedbackInFormFeedBack_Click(object sender, EventArgs e)
        {
            try
            {
                await SendFeedBackAsync();
                await System.Threading.Tasks.Task.Delay(2000);  
                this.Close();
            }
            catch (Exception ex)
            {
                LabelMessageInFormFeedBack.Text = "No se pudo enviar su comentario";
                //AlertUser.Message(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                ButtonSendFeedbackInFormFeedBack.Enabled = true; 
            }
        }

        async Task SendFeedBackAsync()
        {
            ButtonSendFeedbackInFormFeedBack.Enabled = false;
            //_feedBack = new FeedBack(Core.Configuration.StoreLocation, TextBoxUserFeedbackInFormFeedBack.Text);
            LabelMessageInFormFeedBack.Text = "Enviando mensaje...";

            await _feedBack.SendFeedBackAsync();
            LabelMessageInFormFeedBack.BackColor = Color.DarkKhaki;
            LabelMessageInFormFeedBack.Text = "Gracias, su sugerencia fue enviada!";      
        }

        private void FormFeedBack_Load(object sender, EventArgs e)
        {
            DisableButtonSendFeedBackIfTextBoxFeedBackHasInvalidText();                
        }

        void DisableButtonSendFeedBackIfTextBoxFeedBackHasInvalidText()
        {
            ButtonSendFeedbackInFormFeedBack.Enabled = 
                TextBoxUserFeedbackInFormFeedBack.Text == TextBoxUserFeedbackInFormFeedBack.Tag.ToString() 
                ? false 
                : TextBoxUserFeedbackInFormFeedBack.Text == "" ? false : true;
        }
    }

    class FeedBack
    {
        SmtpClient _smtpClient;
        MailMessage _message;
        String _subject, _body;

        public string Subject
        {
            get { return _subject; }
            set { _subject = "MRSES FeedBack - " + value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public FeedBack(string subject, string body)
        {
            _smtpClient = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Timeout = 10000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(Configuration.EmailUserName, Configuration.EmailPassword)
            };

            _message = new MailMessage(Configuration.EmailUserName, Configuration.SendEmailTo)
            {
                Subject = string.Format("MRSHE FeedBack - {0}", subject),
                Body = body
            };
        }

        public async Task SendFeedBackAsync()
        {
            await _smtpClient.SendMailAsync(_message);          
        }
    }
}
