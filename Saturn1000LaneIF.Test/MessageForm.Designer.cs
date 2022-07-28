namespace Vjp.Saturn1000LaneIF.Test
{
    partial class MessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtReceiveMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSendMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbParity = new System.Windows.Forms.Label();
            this.lbBaudRate = new System.Windows.Forms.Label();
            this.lbSerialPort = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtReceiveMessage
            // 
            this.txtReceiveMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtReceiveMessage, "txtReceiveMessage");
            this.txtReceiveMessage.Name = "txtReceiveMessage";
            this.txtReceiveMessage.ReadOnly = true;
            this.txtReceiveMessage.Click += new System.EventHandler(this.txtReceiveMessage_Click);
            this.txtReceiveMessage.MouseLeave += new System.EventHandler(this.txtReceiveMessage_MouseLeave);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtSendMessage
            // 
            this.txtSendMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtSendMessage, "txtSendMessage");
            this.txtSendMessage.Name = "txtSendMessage";
            this.txtSendMessage.Click += new System.EventHandler(this.txtSendMessage_Click);
            this.txtSendMessage.MouseLeave += new System.EventHandler(this.txtSendMessage_MouseLeave);
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.Name = "btnSend";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // btnStartStop
            // 
            resources.ApplyResources(this.btnStartStop, "btnStartStop");
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbParity);
            this.groupBox1.Controls.Add(this.lbBaudRate);
            this.groupBox1.Controls.Add(this.lbSerialPort);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lbParity
            // 
            resources.ApplyResources(this.lbParity, "lbParity");
            this.lbParity.Name = "lbParity";
            // 
            // lbBaudRate
            // 
            resources.ApplyResources(this.lbBaudRate, "lbBaudRate");
            this.lbBaudRate.Name = "lbBaudRate";
            this.lbBaudRate.Click += new System.EventHandler(this.lbBaudRate_Click);
            // 
            // lbSerialPort
            // 
            resources.ApplyResources(this.lbSerialPort, "lbSerialPort");
            this.lbSerialPort.Name = "lbSerialPort";
            // 
            // MessageForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtSendMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtReceiveMessage);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MessageForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtReceiveMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSendMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbSerialPort;
        private System.Windows.Forms.Label lbBaudRate;
        private System.Windows.Forms.Label lbParity;
    }
}

