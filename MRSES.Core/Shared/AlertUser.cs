namespace MRSES.Core.Shared
{
    public struct AlertUser
    {
        public static System.Windows.Forms.DialogResult Message(string messageText, string caption = "")
        {
            return System.Windows.Forms.MessageBox.Show(messageText, caption);
        }

        public static System.Windows.Forms.DialogResult Message(string messageText, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon, System.Windows.Forms.MessageBoxDefaultButton defaultButton)
        {
            return System.Windows.Forms.MessageBox.Show(messageText, caption, buttons, icon, defaultButton);
        }
    }
}
