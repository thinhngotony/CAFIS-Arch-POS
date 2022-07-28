using System;
using System.IO;
using System.Windows.Forms;

namespace Vjp.Saturn1000LaneIF.Test
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            MessageForm messageForm = new MessageForm();
            messageForm.Text = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Application.EnableVisualStyles();
            Application.Run(messageForm);
        }
    }
}
