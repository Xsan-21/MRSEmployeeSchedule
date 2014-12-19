using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRSES.Core.Shared
{
    public static class StringFunctions
    {
        static public bool StringIsNullOrEmpty(string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// This event sets the default text indicator in the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void SetDefaultTextIndicatorInTextBox(TextBox sender)
        {
            if (sender.Text == "" || sender.Text == "\b")
                sender.Text = sender.Tag.ToString();                         
        }

        /// <summary>
        /// This event removes the default text indicator from the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void RemoveDefaultTextIndicator(TextBox sender)
        {
            if (sender.Tag.ToString() != sender.Text)                     
                return;               
            
            sender.Clear();           
        }

        static public void ChangeTextColor(TextBox sender)
        {
            if(sender.Text == sender.Tag.ToString())
                sender.ForeColor = System.Drawing.Color.Gray;
            else
                sender.ForeColor = System.Drawing.Color.Black;
        }
    }
}
