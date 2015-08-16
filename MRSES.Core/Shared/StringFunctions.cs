using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MRSES.Core.Shared
{
    public struct StringFunctions
    {
        static public bool StringIsNullOrEmpty(string text)
        {
            return string.IsNullOrEmpty(text);
        }

        static public bool InputIsValidHourCharacter(char character)
        {
            var result = false;

            if (System.Text.RegularExpressions.Regex.IsMatch(character.ToString(), "^[.0-9apm:\\-\\b]$"))
                result = true;

            return result;
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

        static public string GenerateObjectId(int length, bool accessKey = false)
        {
            var chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            var objectId = string.Empty;
            var random = new Random();

            for (int i = 1; i <= length; i++)
            {
                char _char = chars[random.Next(chars.Length)];
                objectId += accessKey == false ? _char : char.ToUpper(_char);

                if (!accessKey) continue;

                if (i % 5 == 0 && i != length)
                    objectId += "-";
            }

            return objectId;
        }
    }
}
