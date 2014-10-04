using System;
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
        /// This event is used when the user clicks in the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void SetCursorToTheStartOfTextBox(TextBox sender, EventArgs e)
        {
            if (sender.Text == sender.Tag.ToString())
                sender.SelectionStart = 0;
        }

        /// <summary>
        /// This event removes the default text indicator when the user press any key inside a text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void RemoveDefaultTextIndicatorFromTextBox(TextBox sender, KeyPressEventArgs e)
        {
            if (sender.Text == sender.Tag.ToString())
                sender.Text = e.KeyChar.ToString();
            else
                sender.ForeColor = System.Drawing.Color.Black;

            // Workaround
            //============================================================================================
            // for any reason when the user press a key the character is written two times in the text box.
            // This code removes the first duplicated character.
            if (sender.TextLength == 2)
            {
                // do to the problem this is always true, the above condition is used 
                // for remove the duplicated character in the beginning of text.
                if (sender.Text[0] == sender.Text[1])
                {
                    sender.Text = sender.Text.Remove(0, 1);
                    sender.SelectionStart = 1;
                }
            }
        }

        /// <summary>
        /// This event sets the default text indicator if the user leaves the text box empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static public void SetDefaultTextIndicatorIfTextBoxIsEmpty(TextBox sender, EventArgs e)
        {
            if (sender.Text == "" || sender.Text == "\b")
            {
                sender.Text = sender.Tag.ToString();
                sender.ForeColor = System.Drawing.Color.Gray;
            }
        }
    }
}
