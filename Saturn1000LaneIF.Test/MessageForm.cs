using System;
using System.Windows.Forms;
using Vjp.Saturn1000LaneIF.Main;

namespace Vjp.Saturn1000LaneIF.Test
{
    public partial class MessageForm : Form
    {
        public static Serial serial = null;
        private delegate void SafeCallDelegate(string text);
        public MessageForm()
        {
            InitializeComponent();
        }

        public void showMessage(string msg)
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls?view=netframeworkdesktop-4.8
            if (txtReceiveMessage.InvokeRequired)
            {
                var d = new SafeCallDelegate(showMessage);
                txtReceiveMessage.Invoke(d, new object[] { msg });
            }
            else
            {
                txtReceiveMessage.Text = msg;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            serial.SendMsg(txtSendMessage.Text);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (serial == null) serial = new Serial();
            serial.SetCallback(showMessage);
            if (!serial.isPortConnected)
            {
                serial.OpenSerialPort();
                lbSerialPort.Text = "Serial Port: " + serial.PortName;
                lbBaudRate.Text = "Baud Rate: " + serial.BauRate.ToString();
                lbParity.Text = "Parity: " + serial.GetParity().ToString();
                if (serial.isPortConnected)
                {
                    btnStartStop.Text = "Stop";
                    btnSend.Enabled = true;
                }
            }
            else
            {
                serial.CloseSerialPort();
                btnStartStop.Text = "Start";
                btnSend.Enabled = false;
            }
        }

        private bool checkMouseReceive = true;

        private void txtReceiveMessage_Click(object sender, EventArgs e)
        {
            if (checkMouseReceive)
            {
                txtReceiveMessage.SelectAll();
                checkMouseReceive = false;
            }
        }

        private void txtReceiveMessage_MouseLeave(object sender, EventArgs e)
        {
            checkMouseReceive = true;
        }

        private bool checkMouseSend = true;

        private void txtSendMessage_Click(object sender, EventArgs e)
        {
            if (checkMouseSend)
            {
                txtSendMessage.SelectAll();
                checkMouseSend = false;
            }
        }

        private void txtSendMessage_MouseLeave(object sender, EventArgs e)
        {
            checkMouseSend = true;
        }

        private void lbBaudRate_Click(object sender, EventArgs e)
        {

        }
    }
}
