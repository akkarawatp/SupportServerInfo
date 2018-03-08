namespace SupportServerInfo
{
    partial class frmSupportServerInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputFile = new System.Windows.Forms.TextBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbNetworkDevice = new System.Windows.Forms.ComboBox();
            this.lblIPAddrress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMacAddress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Output";
            // 
            // txtOutputFile
            // 
            this.txtOutputFile.Location = new System.Drawing.Point(96, 111);
            this.txtOutputFile.Name = "txtOutputFile";
            this.txtOutputFile.Size = new System.Drawing.Size(539, 20);
            this.txtOutputFile.TabIndex = 1;
            this.txtOutputFile.Text = "D:\\Tmp\\Output.xlsx";
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(96, 137);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 2;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Network Card";
            // 
            // cmbNetworkDevice
            // 
            this.cmbNetworkDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNetworkDevice.FormattingEnabled = true;
            this.cmbNetworkDevice.Location = new System.Drawing.Point(118, 15);
            this.cmbNetworkDevice.Name = "cmbNetworkDevice";
            this.cmbNetworkDevice.Size = new System.Drawing.Size(517, 21);
            this.cmbNetworkDevice.TabIndex = 4;
            this.cmbNetworkDevice.SelectionChangeCommitted += new System.EventHandler(this.cmbNetworkDevice_SelectionChangeCommitted);
            // 
            // lblIPAddrress
            // 
            this.lblIPAddrress.AutoSize = true;
            this.lblIPAddrress.Location = new System.Drawing.Point(144, 41);
            this.lblIPAddrress.Name = "lblIPAddrress";
            this.lblIPAddrress.Size = new System.Drawing.Size(35, 13);
            this.lblIPAddrress.TabIndex = 5;
            this.lblIPAddrress.Text = "label3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "IP :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(328, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Mac Address :";
            // 
            // lblMacAddress
            // 
            this.lblMacAddress.AutoSize = true;
            this.lblMacAddress.Location = new System.Drawing.Point(409, 41);
            this.lblMacAddress.Name = "lblMacAddress";
            this.lblMacAddress.Size = new System.Drawing.Size(35, 13);
            this.lblMacAddress.TabIndex = 8;
            this.lblMacAddress.Text = "label3";
            // 
            // frmSupportServerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 176);
            this.Controls.Add(this.lblMacAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblIPAddrress);
            this.Controls.Add(this.cmbNetworkDevice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.txtOutputFile);
            this.Controls.Add(this.label1);
            this.Name = "frmSupportServerInfo";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmSupportServerInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOutputFile;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbNetworkDevice;
        private System.Windows.Forms.Label lblIPAddrress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMacAddress;
    }
}

