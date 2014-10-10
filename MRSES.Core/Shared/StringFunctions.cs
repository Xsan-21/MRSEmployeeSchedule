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
        async static public Task SetDefaultTextIndicatorInTextBoxAsync(TextBox sender)
        {
            await Task.Delay(0);
            if (sender.Text == "" || sender.Text == "\b")
                sender.Text = sender.Tag.ToString();                         
        }

        /// <summary>
        /// This event removes the default text indicator from the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async static public Task RemoveDefaultTextIndicatorAsync(TextBox sender)
        {
            await Task.Delay(0);
            if (sender.Tag.ToString() != sender.Text)                     
                return;               
            
            sender.Clear();           
        }

        async static public Task ChangeTextColorAsync(TextBox sender)
        {
            await Task.Delay(0);
            if(sender.Text == sender.Tag.ToString())
                sender.ForeColor = System.Drawing.Color.Gray;
            else
                sender.ForeColor = System.Drawing.Color.Black;
        }
    }
}
