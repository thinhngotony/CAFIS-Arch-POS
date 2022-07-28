using System.Windows.Forms;
using Vjp.Skinapp_IF_Test.Common;

namespace Vjp.Skinapp_IF_Test
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStatusCap = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblMsgID = new System.Windows.Forms.Label();
            this.txtMsgID = new System.Windows.Forms.TextBox();
            this.lblSendResult = new System.Windows.Forms.Label();
            this.btnSendResult = new System.Windows.Forms.Button();
            this.lblLastOperator = new System.Windows.Forms.Label();
            this.cboLastOperator = new System.Windows.Forms.ComboBox();
            this.lblPayResult = new System.Windows.Forms.Label();
            this.cboPayResult = new System.Windows.Forms.ComboBox();
            this.lblSettledAmount = new System.Windows.Forms.Label();
            this.txtSettledAmount = new System.Windows.Forms.TextBox();
            this.lblCurrentService = new System.Windows.Forms.Label();
            this.txtCurrentService = new System.Windows.Forms.TextBox();
            this.lblStatementID = new System.Windows.Forms.Label();
            this.txtStatementID = new System.Windows.Forms.TextBox();
            this.txtSendData = new System.Windows.Forms.TextBox();
            this.txtReceiveData = new System.Windows.Forms.TextBox();
            this.txtSettingFile = new System.Windows.Forms.TextBox();
            this.lblSendData = new System.Windows.Forms.Label();
            this.lblReceiveData = new System.Windows.Forms.Label();
            this.lblSettingFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblStatusCap
            // 
            this.lblStatusCap.AutoSize = true;
            this.lblStatusCap.Location = new System.Drawing.Point(14, 22);
            this.lblStatusCap.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatusCap.Name = "lblStatusCap";
            this.lblStatusCap.Size = new System.Drawing.Size(203, 13);
            this.lblStatusCap.TabIndex = 0;
            this.lblStatusCap.Text = "電子マネー決済用アプリ状態：";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(213, 18);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(4);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(227, 20);
            this.txtStatus.TabIndex = 1;
            // 
            // lblMsgID
            // 
            this.lblMsgID.AutoSize = true;
            this.lblMsgID.Location = new System.Drawing.Point(460, 22);
            this.lblMsgID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMsgID.Name = "lblMsgID";
            this.lblMsgID.Size = new System.Drawing.Size(133, 13);
            this.lblMsgID.TabIndex = 2;
            this.lblMsgID.Text = "共通メッセージID：";
            // 
            // txtMsgID
            // 
            this.txtMsgID.Location = new System.Drawing.Point(590, 18);
            this.txtMsgID.Margin = new System.Windows.Forms.Padding(4);
            this.txtMsgID.Name = "txtMsgID";
            this.txtMsgID.ReadOnly = true;
            this.txtMsgID.Size = new System.Drawing.Size(117, 20);
            this.txtMsgID.TabIndex = 3;
            // 
            // lblSendResult
            // 
            this.lblSendResult.AutoSize = true;
            this.lblSendResult.Location = new System.Drawing.Point(14, 83);
            this.lblSendResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSendResult.Name = "lblSendResult";
            this.lblSendResult.Size = new System.Drawing.Size(105, 13);
            this.lblSendResult.TabIndex = 4;
            this.lblSendResult.Text = "■取引結果通知";
            // 
            // btnSendResult
            // 
            this.btnSendResult.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnSendResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendResult.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSendResult.Location = new System.Drawing.Point(18, 115);
            this.btnSendResult.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendResult.Name = "btnSendResult";
            this.btnSendResult.Size = new System.Drawing.Size(111, 35);
            this.btnSendResult.TabIndex = 5;
            this.btnSendResult.Text = "結果通知";
            this.btnSendResult.UseVisualStyleBackColor = false;
            this.btnSendResult.Click += new System.EventHandler(this.BtnSendResult_Click);
            // 
            // lblLastOperator
            // 
            this.lblLastOperator.AutoSize = true;
            this.lblLastOperator.Location = new System.Drawing.Point(14, 182);
            this.lblLastOperator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastOperator.Name = "lblLastOperator";
            this.lblLastOperator.Size = new System.Drawing.Size(91, 13);
            this.lblLastOperator.TabIndex = 6;
            this.lblLastOperator.Text = "最終操作者：";
            // 
            // cboLastOperator
            // 
            this.cboLastOperator.FormattingEnabled = true;
            this.cboLastOperator.ItemHeight = 13;
            this.cboLastOperator.Location = new System.Drawing.Point(129, 178);
            this.cboLastOperator.Margin = new System.Windows.Forms.Padding(4);
            this.cboLastOperator.Name = "cboLastOperator";
            this.cboLastOperator.Size = new System.Drawing.Size(160, 21);
            this.cboLastOperator.TabIndex = 7;
            // 
            // lblPayResult
            // 
            this.lblPayResult.AutoSize = true;
            this.lblPayResult.Location = new System.Drawing.Point(14, 221);
            this.lblPayResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPayResult.Name = "lblPayResult";
            this.lblPayResult.Size = new System.Drawing.Size(49, 13);
            this.lblPayResult.TabIndex = 8;
            this.lblPayResult.Text = "結果：";
            // 
            // cboPayResult
            // 
            this.cboPayResult.FormattingEnabled = true;
            this.cboPayResult.ItemHeight = 13;
            this.cboPayResult.Location = new System.Drawing.Point(129, 217);
            this.cboPayResult.Margin = new System.Windows.Forms.Padding(4);
            this.cboPayResult.Name = "cboPayResult";
            this.cboPayResult.Size = new System.Drawing.Size(160, 21);
            this.cboPayResult.TabIndex = 9;
            // 
            // lblSettledAmount
            // 
            this.lblSettledAmount.AutoSize = true;
            this.lblSettledAmount.Location = new System.Drawing.Point(14, 260);
            this.lblSettledAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSettledAmount.Name = "lblSettledAmount";
            this.lblSettledAmount.Size = new System.Drawing.Size(77, 13);
            this.lblSettledAmount.TabIndex = 10;
            this.lblSettledAmount.Text = "決済金額：";
            // 
            // txtSettledAmount
            // 
            this.txtSettledAmount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSettledAmount.Location = new System.Drawing.Point(129, 256);
            this.txtSettledAmount.Margin = new System.Windows.Forms.Padding(4);
            this.txtSettledAmount.MaxLength = 12;
            this.txtSettledAmount.Name = "txtSettledAmount";
            this.txtSettledAmount.Size = new System.Drawing.Size(160, 20);
            this.txtSettledAmount.TabIndex = 11;
            this.txtSettledAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSettledAmount.Click += new System.EventHandler(this.txtSettledAmount_Click);
            this.txtSettledAmount.GotFocus += new System.EventHandler(this.txtSettledAmount_GotFocus);
            this.txtSettledAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSettledAmount_KeyPress);
            this.txtSettledAmount.MouseLeave += new System.EventHandler(this.txtSettledAmount_MouseLeave);
            // 
            // lblCurrentService
            // 
            this.lblCurrentService.AutoSize = true;
            this.lblCurrentService.Location = new System.Drawing.Point(14, 303);
            this.lblCurrentService.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentService.Name = "lblCurrentService";
            this.lblCurrentService.Size = new System.Drawing.Size(119, 13);
            this.lblCurrentService.TabIndex = 12;
            this.lblCurrentService.Text = "決済サービス名：";
            // 
            // txtCurrentService
            // 
            this.txtCurrentService.Location = new System.Drawing.Point(129, 299);
            this.txtCurrentService.Margin = new System.Windows.Forms.Padding(4);
            this.txtCurrentService.Name = "txtCurrentService";
            this.txtCurrentService.Size = new System.Drawing.Size(160, 20);
            this.txtCurrentService.TabIndex = 13;
            this.txtCurrentService.Click += new System.EventHandler(this.txtCurrentService_Click);
            this.txtCurrentService.GotFocus += new System.EventHandler(this.txtCurrentService_GotFocus);
            this.txtCurrentService.MouseLeave += new System.EventHandler(this.txtCurrentService_MouseLeave);
            // 
            // lblStatementID
            // 
            this.lblStatementID.AutoSize = true;
            this.lblStatementID.Location = new System.Drawing.Point(14, 342);
            this.lblStatementID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatementID.Name = "lblStatementID";
            this.lblStatementID.Size = new System.Drawing.Size(77, 13);
            this.lblStatementID.TabIndex = 14;
            this.lblStatementID.Text = "取引通番：";
            // 
            // txtStatementID
            // 
            this.txtStatementID.Location = new System.Drawing.Point(129, 339);
            this.txtStatementID.Margin = new System.Windows.Forms.Padding(4);
            this.txtStatementID.Name = "txtStatementID";
            this.txtStatementID.Size = new System.Drawing.Size(160, 20);
            this.txtStatementID.TabIndex = 15;
            this.txtStatementID.Click += new System.EventHandler(this.txtStatementID_Click);
            this.txtStatementID.GotFocus += new System.EventHandler(this.txtStatementID_GotFocus);
            this.txtStatementID.MouseLeave += new System.EventHandler(this.txtStatementID_MouseLeave);
            // 
            // txtSendData
            // 
            this.txtSendData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSendData.Location = new System.Drawing.Point(305, 143);
            this.txtSendData.Margin = new System.Windows.Forms.Padding(4);
            this.txtSendData.Multiline = true;
            this.txtSendData.Name = "txtSendData";
            this.txtSendData.ReadOnly = true;
            this.txtSendData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSendData.Size = new System.Drawing.Size(440, 628);
            this.txtSendData.TabIndex = 16;
            this.txtSendData.Click += new System.EventHandler(this.txtSendData_Click);
            this.txtSendData.MouseLeave += new System.EventHandler(this.txtSendData_MouseLeave);
            // 
            // txtReceiveData
            // 
            this.txtReceiveData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceiveData.Location = new System.Drawing.Point(766, 143);
            this.txtReceiveData.Margin = new System.Windows.Forms.Padding(4);
            this.txtReceiveData.Multiline = true;
            this.txtReceiveData.Name = "txtReceiveData";
            this.txtReceiveData.ReadOnly = true;
            this.txtReceiveData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtReceiveData.Size = new System.Drawing.Size(440, 628);
            this.txtReceiveData.TabIndex = 17;
            this.txtReceiveData.Click += new System.EventHandler(this.txtReceiveData_Click);
            this.txtReceiveData.MouseLeave += new System.EventHandler(this.txtReceiveData_MouseLeave);
            // 
            // txtSettingFile
            // 
            this.txtSettingFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSettingFile.Location = new System.Drawing.Point(1227, 63);
            this.txtSettingFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtSettingFile.Multiline = true;
            this.txtSettingFile.Name = "txtSettingFile";
            this.txtSettingFile.ReadOnly = true;
            this.txtSettingFile.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSettingFile.Size = new System.Drawing.Size(290, 691);
            this.txtSettingFile.TabIndex = 18;
            // 
            // lblSendData
            // 
            this.lblSendData.AutoSize = true;
            this.lblSendData.Location = new System.Drawing.Point(482, 115);
            this.lblSendData.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSendData.Name = "lblSendData";
            this.lblSendData.Size = new System.Drawing.Size(105, 13);
            this.lblSendData.TabIndex = 19;
            this.lblSendData.Text = "送信データ表示";
            // 
            // lblReceiveData
            // 
            this.lblReceiveData.AutoSize = true;
            this.lblReceiveData.Location = new System.Drawing.Point(932, 115);
            this.lblReceiveData.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReceiveData.Name = "lblReceiveData";
            this.lblReceiveData.Size = new System.Drawing.Size(105, 13);
            this.lblReceiveData.TabIndex = 20;
            this.lblReceiveData.Text = "受信データ表示";
            // 
            // lblSettingFile
            // 
            this.lblSettingFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSettingFile.AutoSize = true;
            this.lblSettingFile.Location = new System.Drawing.Point(1296, 46);
            this.lblSettingFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSettingFile.Name = "lblSettingFile";
            this.lblSettingFile.Size = new System.Drawing.Size(147, 13);
            this.lblSettingFile.TabIndex = 21;
            this.lblSettingFile.Text = "設定ファイル内容表示";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1530, 816);
            this.Controls.Add(this.lblSettingFile);
            this.Controls.Add(this.lblReceiveData);
            this.Controls.Add(this.lblSendData);
            this.Controls.Add(this.txtSettingFile);
            this.Controls.Add(this.txtReceiveData);
            this.Controls.Add(this.txtSendData);
            this.Controls.Add(this.txtStatementID);
            this.Controls.Add(this.lblStatementID);
            this.Controls.Add(this.txtCurrentService);
            this.Controls.Add(this.lblCurrentService);
            this.Controls.Add(this.txtSettledAmount);
            this.Controls.Add(this.lblSettledAmount);
            this.Controls.Add(this.cboPayResult);
            this.Controls.Add(this.lblPayResult);
            this.Controls.Add(this.cboLastOperator);
            this.Controls.Add(this.lblLastOperator);
            this.Controls.Add(this.btnSendResult);
            this.Controls.Add(this.lblSendResult);
            this.Controls.Add(this.txtMsgID);
            this.Controls.Add(this.lblMsgID);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lblStatusCap);
            this.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = GlobalData.AppName;
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatusCap;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblMsgID;
        private System.Windows.Forms.TextBox txtMsgID;
        private System.Windows.Forms.Label lblSendResult;
        private System.Windows.Forms.Button btnSendResult;
        private Label lblLastOperator;
        private ComboBox cboLastOperator;
        private Label lblPayResult;
        private ComboBox cboPayResult;
        private Label lblSettledAmount;
        private TextBox txtSettledAmount;
        private Label lblCurrentService;
        private TextBox txtCurrentService;
        private Label lblStatementID;
        private TextBox txtStatementID;
        private TextBox txtSendData;
        private TextBox txtReceiveData;
        private TextBox txtSettingFile;
        private Label lblSendData;
        private Label lblReceiveData;
        private Label lblSettingFile;
    }
}

