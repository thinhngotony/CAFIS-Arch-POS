using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Timers;
using Vjp.Skinapp_IF_Test.Common;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Vjp.Skinapp_IF_Test
{
    public partial class MainForm : Form
    {
        private int CustomMessageID = 0;
        private string CustomMessageName = "";
        private static Timer workingTimer;
        private bool checkMouseSendData = true;
        private bool checkMouseSettledAmount = true;
        private bool checkMouseReceiveData = true;
        private bool checkFocusCurrentService = true;
        private bool checkFocusStatementID = true;

        public MainForm()
        {
            InitializeComponent();
            InitComboboxes();
            FormatItems();
            CalculateTextBoxSizes();
            RegisterWindowMessage();
            GlobalData.CurrentState = Utilities.State.Idle;
            GlobalData.SecondState = "非表示";
            ReadConfig();
            UpdateState();
        }

        protected override void OnShown(EventArgs e)
        {
            WinAPI.HideWindow();
        }

        private void RegisterWindowMessage()
        {
            CustomMessageName = GlobalData.PosIFConfig.msg_name;
            CustomMessageID = WinAPI.RegisterWindowMessage(CustomMessageName);
            txtMsgID.Text = CustomMessageID.ToString();
            Utilities.Log.Info("CustomMessageID: " + CustomMessageID + " | CustomMessageName: " + CustomMessageName);
        }

        private void ReadConfig()
        {
            string tempText = "msg_name: " + GlobalData.PosIFConfig.msg_name + Environment.NewLine + Environment.NewLine;
            tempText += "pos_prosess_name: " + GlobalData.PosIFConfig.pos_process_name + Environment.NewLine + Environment.NewLine;
            tempText += "pos_window_name: " + GlobalData.PosIFConfig.pos_window_name + Environment.NewLine + Environment.NewLine;
            tempText += "pos_btn_loc: [" + GlobalData.PosIFConfig.pos_btn_loc[0] + ", " + GlobalData.PosIFConfig.pos_btn_loc[1] + "]" + Environment.NewLine + Environment.NewLine;
            tempText += "pos_timeout: " + GlobalData.PosIFConfig.pos_timeout.ToString() + Environment.NewLine + Environment.NewLine;
            tempText += "from_pos_dat: " + GlobalData.PosIFConfig.from_pos_dat + Environment.NewLine + Environment.NewLine;
            tempText += "to_pos_dat: " + GlobalData.PosIFConfig.to_pos_dat + Environment.NewLine;
            txtSettingFile.Text += tempText + Environment.NewLine;
            Utilities.Log.Info(tempText);
        }
        public void FormPos()
        {
            Utilities.CsFileRead(GlobalData.AppPath, GlobalData.PosIFConfig.from_pos_dat, ref GlobalData.Data_FromPosDat);
            string tempText = "{service: " + GlobalData.Data_FromPosDat.service;
            tempText += ", sequence: " + GlobalData.Data_FromPosDat.sequence;
            tempText += ", location: " + GlobalData.Data_FromPosDat.location;
            tempText += ", amount: " + GlobalData.Data_FromPosDat.amount;
            tempText += ", regi_tran_date: " + GlobalData.Data_FromPosDat.regi_tran_date;
            tempText += ", customer_timeout: " + GlobalData.Data_FromPosDat.customer_timeout + "}";
            // txtReceiveData.Text += "payment_method: " + GlobalData.Data_FromPosDat.payment_method + endLine();
            // txtReceiveData.Text += "tran_type: " + GlobalData.Data_FromPosDat.tran_type + endLine();
            // txtReceiveData.Text += "receipt_num: " + GlobalData.Data_FromPosDat.receipt_num + endLine();
            WriteReceiveMessage(tempText, "FromPos");
            Utilities.Log.Info(tempText);
        }
        public void ToPos()
        {
            Utilities.CsFileRead(GlobalData.AppPath, GlobalData.PosIFConfig.to_pos_dat, ref GlobalData.Data_ToPosDat);
            string tempText = "{service: " + GlobalData.Data_ToPosDat.service;
            tempText += ", sequence: " + GlobalData.Data_ToPosDat.sequence;
            tempText += ", last_operator: " + GlobalData.Data_ToPosDat.last_operator;
            tempText += ", result: " + GlobalData.Data_ToPosDat.result;
            tempText += ", SettledAmount: " + GlobalData.Data_ToPosDat.SettledAmount;
            tempText += ", CurrentService: " + GlobalData.Data_ToPosDat.CurrentService;
            tempText += ", statementID: " + GlobalData.Data_ToPosDat.statementID + "}";
            // txtSendData.Text += "payment_method: " + GlobalData.Data_ToPosDat.payment_method + endLine();
            // txtSendData.Text += "SlipNumber: " + GlobalData.Data_ToPosDat.SlipNumber + endLine();
            // txtSendData.Text += "TransactionNumber: " + GlobalData.Data_ToPosDat.TransactionNumber + endLine();
            // txtSendData.Text += "TransactionType: " + GlobalData.Data_ToPosDat.TransactionType + endLine();
            WriteSendMessage(tempText, "ToPos");
            Utilities.Log.Info(tempText);
        }

        // DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff")
        public void WriteSendMessage(string msg, string methodName = "")
        {
            string tempMethod = methodName != "" ? " | " + methodName + " |" : "";
            txtSendData.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + tempMethod + " " + msg + Environment.NewLine + Environment.NewLine);
        }
        public void WriteReceiveMessage(string msg, string methodName = "")
        {
            string tempMethod = methodName != "" ? " | " + methodName + " |" : "";
            txtReceiveData.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + tempMethod + " " + msg + Environment.NewLine + Environment.NewLine);
        }
        private void InitComboboxes()
        {
            List<ComboboxItem> items = new List<ComboboxItem>
            {
                new ComboboxItem() { Text = "0:店員操作", Value = 0 },
                new ComboboxItem() { Text = "1:消費者操作", Value = 1 }
            };
            cboLastOperator.DataSource = items;
            cboLastOperator.DisplayMember = "Text";
            cboLastOperator.ValueMember = "Value";
            cboLastOperator.SelectedIndex = 1;

            List<ComboboxItem> items2 = new List<ComboboxItem>
            {
                new ComboboxItem() { Text = "0:中止", Value = 0 },
                new ComboboxItem() { Text = "1:支払成功", Value = 1 },
                new ComboboxItem() { Text = "9:消費者操作タイムアウト", Value = 9 }
            };
            cboPayResult.DataSource = items2;
            cboPayResult.DisplayMember = "Text";
            cboPayResult.ValueMember = "Value";
            cboPayResult.SelectedIndex = 1;
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString()
            {
                return Text;
            }
        }
        // Add hook to receive message from window procedure
        private void MainForm_Load(object sender, EventArgs e)
        {
            //HwndSource source = HwndSource.FromHwnd(this.Handle);
            //source.AddHook(new HwndSourceHook(WndProc));
        }

        private async void BtnSendResult_Click(object sender, EventArgs e)
        {
            GlobalData.Data_ToPosDat.service = GlobalData.Data_FromPosDat.service;
            GlobalData.Data_ToPosDat.sequence = GlobalData.Data_FromPosDat.sequence;
            GlobalData.Data_ToPosDat.last_operator = (cboLastOperator.SelectedItem as ComboboxItem).Value.ToString();
            GlobalData.Data_ToPosDat.result = (cboPayResult.SelectedItem as ComboboxItem).Value.ToString();
            GlobalData.Data_ToPosDat.SettledAmount = txtSettledAmount.Text.Replace(",", String.Empty);
            GlobalData.Data_ToPosDat.CurrentService = txtCurrentService.Text;
            GlobalData.Data_ToPosDat.statementID = txtStatementID.Text;
            await PostMessage();
            UpdateState();
        }

        // https://stackoverflow.com/questions/10820502/unable-to-use-hwndsource-in-my-code
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.wndproc?redirectedfrom=MSDN&view=netframework-4.7.2
        protected override void WndProc(ref Message m) {
            if (m.Msg == CustomMessageID)
            {
                ReceiveMessage(ref m);
            }
            base.WndProc(ref m);
        }
        protected void ReceiveMessage(ref Message m) {
            WriteReceiveMessage("HWND: " + m.HWnd + ", UINT: " + m.Msg + ", WParam: " + m.WParam.ToInt32() + ", LParam: " + m.LParam.ToInt32(), "ReceiveMessage");
            Utilities.Log.Info("HWND: " + m.HWnd + ", UINT: " + m.Msg + ", WParam: " + m.WParam.ToInt32() + ", LParam: " + m.LParam.ToInt32());
            switch (m.WParam.ToInt32())
            {
                case 1: // Confirm status
                    if (GlobalData.CurrentState == Utilities.State.Idle)
                    {
                        m.Result = (IntPtr)1;
                    }
                    else if (GlobalData.CurrentState == Utilities.State.Transacting || GlobalData.CurrentState == Utilities.State.Maintaining)
                    {
                        m.Result = (IntPtr)2;
                    }
                    else
                    {
                        m.Result = (IntPtr)9;
                    }
                    break;
                case 2: // Read FromPos.dat and start transaction
                    bool readSuccess = Utilities.CsFileRead(GlobalData.AppPath, GlobalData.FromPosPath, ref GlobalData.Data_FromPosDat);
                    (int res, string msg) validFromPos = Utilities.ValidateFromPos(GlobalData.Data_FromPosDat);
                    if (GlobalData.CurrentState == Utilities.State.Idle)
                    {
                        if (readSuccess && validFromPos.res == 1)
                        {
                            m.Result = (IntPtr)1;
                            FormPos();
                            txtSettledAmount.Text = GlobalData.Data_FromPosDat.amount.ToString();
                            GlobalData.CurrentState = Utilities.State.Transacting;
                            if (GlobalData.Data_FromPosDat.location == "0")
                            {
                                GlobalData.CurrentState = Utilities.State.Maintaining;
                            }
                        }
                        else if (validFromPos.res == 3)
                        {
                            m.Result = (IntPtr)3;
                            WriteReceiveMessage(validFromPos.msg, "ReceiveMessage");
                        }
                        else
                        {
                            m.Result = (IntPtr)9;
                            WriteReceiveMessage(validFromPos.msg, "ReceiveMessage");
                        }
                    }
                    else
                    {
                        m.Result = (IntPtr)9;
                    }
                    if (File.Exists(GlobalData.AppPath + GlobalData.FromPosPath))
                    {
                        File.Delete(GlobalData.AppPath + GlobalData.FromPosPath);
                    }
                    break;
                case 9: // End add-on app
                    if (GlobalData.CurrentState == Utilities.State.Idle)
                    {
                        m.Result = (IntPtr)1;
                        // GlobalData.CurrentState = Utilities.State.Closed;
                        this.Close();
                    }
                    else
                    {
                        m.Result = (IntPtr)9;
                    }
                    break;
                default:
                    int retVal = m.WParam.ToInt32() + m.LParam.ToInt32();
                    m.Result = (IntPtr)retVal;
                    break;
            }
            WriteSendMessage("Result: " + m.Result.ToInt32(), "ReceiveMessage");
            Utilities.Log.Info("Result: " + m.Result.ToInt32());
            WinAPI.ReplyMessage(m.Result);
            UpdateState();
        }

        public async Task PostMessage()
        {
            Utilities.PosIFConfig config = GlobalData.PosIFConfig;

            Utilities.CsFileWrite(GlobalData.AppPath, GlobalData.ToPosPath, GlobalData.Data_ToPosDat);

            ToPos();

            ReceiveTimerStart(OnWaitingNotifyTransactionResultResponseTimeOver, config.pos_timeout);

            // Send mouse click event to the button on regi-app.
            bool res = true;
            int[] location = config.pos_btn_loc;
            int hWnd = WinAPI.FindWindow(null, config.pos_window_name);
            // Find pos process
            Process[] posProcess = Process.GetProcessesByName(config.pos_process_name);
            if (0 == posProcess.Length)
            {
                Utilities.Log.Error("相手プロセスが取得できません");
                res = false;
            }

            // Find pos window
            if (hWnd == 0)
            {
                Utilities.Log.Error("相手Windowのハンドルが取得できません");
                res = false;
            }

            if (res)
            {
                // await System.Threading.Tasks.Task.Delay(6000);
                WriteSendMessage("Send mouse click event with position: x: " + location[0] + "; y: " + location[1], "PostMessage");
                Utilities.Log.Info("Send mouse click event with position: x: " + location[0] + "; y: " + location[1]);
                int mouseDown = WinAPI.PostMessage(hWnd, WinAPI.WM_LBUTTONDOWN, WinAPI.MK_LBUTTON, Utilities.GetLoc(location[0], location[1]));
                WriteSendMessage("HWND: " + hWnd + ", UINT: " + WinAPI.WM_LBUTTONDOWN + ", WParam: " + WinAPI.MK_LBUTTON + ", LParam: " + Utilities.GetLoc(location[0], location[1]), "PostMessage");
                Utilities.Log.Info("HWND: " + hWnd + ", UINT: " + WinAPI.WM_LBUTTONDOWN + ", WParam: " + WinAPI.MK_LBUTTON + ", LParam: " + Utilities.GetLoc(location[0], location[1]));
                int mouseUp = WinAPI.PostMessage(hWnd, WinAPI.WM_LBUTTONUP, WinAPI.MK_LBUTTON, Utilities.GetLoc(location[0], location[1]));
                WriteSendMessage("HWND: " + hWnd + ", UINT: " + WinAPI.WM_LBUTTONUP + ", WParam: " + WinAPI.MK_LBUTTON + ", LParam: " + Utilities.GetLoc(location[0], location[1]), "PostMessage");
                Utilities.Log.Info("HWND: " + hWnd + ", UINT: " + WinAPI.WM_LBUTTONUP + ", WParam: " + WinAPI.MK_LBUTTON + ", LParam: " + Utilities.GetLoc(location[0], location[1]));
                res = mouseDown == 1 && mouseUp == 1;
            }

            ReceiveTimerStop();

            GlobalData.CurrentState = Utilities.State.Idle;

            if (res)
            {
                WriteReceiveMessage("Result: 1", "PostMessage");
                Utilities.Log.Info("Result: 1");
                GlobalData.SecondState = "非表示";
            }
            else
            {
                WriteReceiveMessage("Result: 0", "PostMessage");
                Utilities.Log.Info("Result: 0");
                GlobalData.SecondState = "非表示";
            }

            // Wait for hiding and unlocking topmost by regi-app.
            int timeout = config.pos_timeout;
            bool isHidden = false;
            IntPtr hwnd = WinAPI.FindWindowByCaption(IntPtr.Zero, GlobalData.AppName);
            while (timeout >= 100)
            {
                await Task.Delay(100);
                timeout -= 100;
                if (!WinAPI.IsWindowTopMost(hwnd))
                {
                    isHidden = true;
                    break;
                }
            }

            // Auto-hide app if timeout.
            if (!isHidden)
            {
                Utilities.Log.Info("Auto-hide Skin App because of timeout.");
                WinAPI.HideWindow();
            }
            //Session.ScreenState.NextState = StateMachine.State.emI;
            Utilities.Log.Info("操作完了");
        }

        private void UpdateState()
        {
            switch (GlobalData.CurrentState)
            {
                case Utilities.State.Idle:
                    txtStatus.Text = "待機中 (" + GlobalData.SecondState + ")";
                    btnSendResult.Enabled = false;
                    btnSendResult.BackColor = Color.CornflowerBlue;
                    break;
                case Utilities.State.Transacting:
                    GlobalData.SecondState = "表示";
                    txtStatus.Text = "取引中 (" + GlobalData.SecondState + ")";
                    btnSendResult.Enabled = true;
                    btnSendResult.BackColor = Color.Blue;
                    break;
                case Utilities.State.Maintaining:
                    GlobalData.SecondState = "表示";
                    txtStatus.Text = "メンテ呼出中 (" + GlobalData.SecondState + ")";
                    btnSendResult.Enabled = true;
                    btnSendResult.BackColor = Color.Blue;
                    break;
                case Utilities.State.Closed:
                    GlobalData.SecondState = "非表示";
                    txtStatus.Text = "アプリ終了 (" + GlobalData.SecondState + ")";
                    btnSendResult.Enabled = false;
                    btnSendResult.BackColor = Color.CornflowerBlue;
                    break;
            }
        }

        void ReceiveTimerStart(ElapsedEventHandler handler, int timeOut)
        {
            // Stop the running timer
            if (workingTimer != null)
            {
                workingTimer.Stop();
            }
            workingTimer = new Timer(timeOut);
            workingTimer.AutoReset = false;
            workingTimer.Elapsed += handler;
            // Timer start
            workingTimer.Start();
        }

        // Receive timer stopped
        void ReceiveTimerStop()
        {
            if (workingTimer != null)
            {
                workingTimer.Stop();
                workingTimer = null;
            }
        }

        void OnWaitingNotifyTransactionResultResponseTimeOver(object sender, ElapsedEventArgs e)
        {
            // Stop timer
            Timer timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
            }
            WriteSendMessage("Time out when waiting response after transaction!", "OnWaitingNotifyTransactionResultResponseTimeOver");
            Utilities.Log.Error("Time out when waiting response after transaction!");
            GlobalData.CurrentState = Utilities.State.Idle;
            GlobalData.SecondState = "非表示";
        }

        private void FormatItems()
        {
            txtSettledAmount.Text = "0";
        }

        private void txtSendData_Click(object sender, EventArgs e)
        {
            if (checkMouseSendData)
            {
                txtSendData.SelectAll();
                checkMouseSendData = false;
            }
        }
        private void txtSendData_MouseLeave(object sender, EventArgs e)
        {
            checkMouseSendData = true;
        }

        private void txtReceiveData_Click(object sender, EventArgs e)
        {
            if (checkMouseReceiveData)
            {
                txtReceiveData.SelectAll();
                checkMouseReceiveData = false;
            }
        }
        private void txtReceiveData_MouseLeave(object sender, EventArgs e)
        {
            checkMouseReceiveData = true;
        }

        private void txtSettledAmount_Click(object sender, EventArgs e)
        {
            if (checkMouseSettledAmount)
            {
                txtSettledAmount.SelectAll();
                checkMouseSettledAmount = false;
            }
        }
        private void txtSettledAmount_GotFocus(object sender, EventArgs e)
        {
            txtSettledAmount.SelectAll();
        }
        private void txtSettledAmount_MouseLeave(object sender, EventArgs e)
        {
            checkMouseSettledAmount = true;
        }
        private void txtSettledAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCurrentService_Click(object sender, EventArgs e)
        {
            if (checkFocusCurrentService)
            {
                txtCurrentService.SelectAll();
                checkFocusCurrentService = false;
            }
        }
        private void txtCurrentService_GotFocus(object sender, EventArgs e)
        {
            txtCurrentService.SelectAll();
        }
        private void txtCurrentService_MouseLeave(object sender, EventArgs e)
        {
            checkFocusCurrentService = true;
        }

        private void txtStatementID_Click(object sender, EventArgs e)
        {
            if (checkFocusStatementID)
            {
                txtStatementID.SelectAll();
                checkFocusStatementID = false;
            }
        }
        private void txtStatementID_GotFocus(object sender, EventArgs e)
        {
            txtStatementID.SelectAll();
        }
        private void txtStatementID_MouseLeave(object sender, EventArgs e)
        {
            checkFocusStatementID = true;
        }

        /*private void txtSettledAmount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(txtSettledAmount.Text, "[^0-9,]") || txtSettledAmount.Text == "")
                {
                    txtSettledAmount.Text = "0";
                }
                var a = double.Parse(txtSettledAmount.Text);
                txtSettledAmount.Text = String.Format("{0:n0}", a);
            }
            catch (Exception ex)
            {
                Utilities.log.Error(ex.Message);
                txtSettledAmount.Text = "0";
            }
        }*/

        private void MainForm_Resize(object sender, EventArgs e)
        {
            CalculateTextBoxSizes();
        }

        private void CalculateTextBoxSizes()
        {
            txtSendData.Width = (txtSettingFile.Location.X - txtSendData.Location.X - 20 - 20) / 2;
            txtReceiveData.Location = new System.Drawing.Point(txtSendData.Location.X + 20 + txtSendData.Width, 143);
            txtReceiveData.Width = txtSendData.Width;
            lblSendData.Location = new System.Drawing.Point(txtSendData.Location.X + txtSendData.Width / 2 - lblSendData.Width / 2, 115);
            lblReceiveData.Location = new System.Drawing.Point(txtSendData.Location.X + txtSendData.Width + 20 + txtReceiveData.Width / 2 - lblReceiveData.Width / 2, 115);
        }

        // Handle and reply to messages from pos
        /*private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Message m = Message.Create(hwnd, msg, wParam, lParam);
            if (m.Msg == _rgtrMsg)
            {
                switch (m.WParam.ToInt32())
                {
                    case 1: // Confirm status
                        if (GlobalData.CurrentState == Utilities.State.emI)
                        {
                            m.Result = (IntPtr)1;
                            Session.IsStatusOK = true;
                            GlobalData.CurrentState = Utilities.State.emS;
                        }
                        else if (GlobalData.CurrentState != Utilities.State.emMAorMI_EMSG)
                        {
                            m.Result = (IntPtr)2;
                            Session.IsStatusOK = false;
                        }
                        else
                        {
                            m.Result = (IntPtr)9;
                            Session.IsStatusOK = false;
                        }
                        break;
                    case 2: // Read FromPos.dat and start transaction
                        bool readSuccess = Utilities.CsFileRead(GlobalData.AppPath, GlobalData.FromPosPath, ref GlobalData.Data_FromPosDat);
                        bool containNull = Utilities.ContainNull(GlobalData.Data_FromPosDat);
                        if (readSuccess && !containNull)
                        {
                            m.Result = (IntPtr)1;
                            Session.IsTransactionStart = true;
                        }
                        else
                        {
                            m.Result = (IntPtr)3;
                            Session.IsTransactionStart = false;
                        }
                        if (File.Exists(GlobalData.AppPath + GlobalData.FromPosPath))
                        {
                            File.Delete(GlobalData.AppPath + GlobalData.FromPosPath);
                        }
                        break;
                    case 9: // End add-on app
                        m.Result = (IntPtr)1;
                        this.Close();
                        break;
                    default:
                        int retVal = m.WParam.ToInt32() + m.LParam.ToInt32();
                        m.Result = (IntPtr)retVal;
                        break;
                }
                WinAPI.ReplyMessage(m.Result);
            }
            return IntPtr.Zero;
        }*/
    }
}
