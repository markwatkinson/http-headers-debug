namespace HTTP
{
    partial class Http
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
            this.InputTxt = new System.Windows.Forms.TextBox();
            this.GoBtn = new System.Windows.Forms.Button();
            this.errorLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hostLbl = new System.Windows.Forms.Label();
            this.summaryOutTab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.otherInformationLbl = new System.Windows.Forms.Label();
            this.statusCodeLbl = new System.Windows.Forms.Label();
            this.httpVersionLbl = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.rawOutTab = new System.Windows.Forms.TabPage();
            this.OutputTxt = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.summaryOutTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.rawOutTab.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // InputTxt
            // 
            this.InputTxt.Location = new System.Drawing.Point(9, 32);
            this.InputTxt.Multiline = true;
            this.InputTxt.Name = "InputTxt";
            this.InputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InputTxt.Size = new System.Drawing.Size(359, 177);
            this.InputTxt.TabIndex = 0;
            this.InputTxt.WordWrap = false;
            this.InputTxt.TextChanged += new System.EventHandler(this.InputTxt_TextChanged);
            // 
            // GoBtn
            // 
            this.GoBtn.Location = new System.Drawing.Point(354, 316);
            this.GoBtn.Name = "GoBtn";
            this.GoBtn.Size = new System.Drawing.Size(75, 23);
            this.GoBtn.TabIndex = 2;
            this.GoBtn.Text = "Go";
            this.GoBtn.UseVisualStyleBackColor = true;
            this.GoBtn.Click += new System.EventHandler(this.GoBtn_Click);
            // 
            // errorLbl
            // 
            this.errorLbl.AutoSize = true;
            this.errorLbl.ForeColor = System.Drawing.Color.Red;
            this.errorLbl.Location = new System.Drawing.Point(6, 225);
            this.errorLbl.Name = "errorLbl";
            this.errorLbl.Size = new System.Drawing.Size(58, 13);
            this.errorLbl.TabIndex = 3;
            this.errorLbl.Text = "Error Label";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Request:";
            // 
            // hostLbl
            // 
            this.hostLbl.AutoSize = true;
            this.hostLbl.Location = new System.Drawing.Point(53, 16);
            this.hostLbl.Name = "hostLbl";
            this.hostLbl.Size = new System.Drawing.Size(64, 13);
            this.hostLbl.TabIndex = 5;
            this.hostLbl.Text = "example.org";
            // 
            // summaryOutTab
            // 
            this.summaryOutTab.Controls.Add(this.groupBox1);
            this.summaryOutTab.Controls.Add(this.statusCodeLbl);
            this.summaryOutTab.Controls.Add(this.httpVersionLbl);
            this.summaryOutTab.Controls.Add(this.label3);
            this.summaryOutTab.Controls.Add(this.label2);
            this.summaryOutTab.Location = new System.Drawing.Point(4, 22);
            this.summaryOutTab.Name = "summaryOutTab";
            this.summaryOutTab.Padding = new System.Windows.Forms.Padding(3);
            this.summaryOutTab.Size = new System.Drawing.Size(402, 219);
            this.summaryOutTab.TabIndex = 1;
            this.summaryOutTab.Text = "Summary";
            this.summaryOutTab.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.otherInformationLbl);
            this.groupBox1.Location = new System.Drawing.Point(10, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 173);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Other Information";
            // 
            // otherInformationLbl
            // 
            this.otherInformationLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.otherInformationLbl.Location = new System.Drawing.Point(3, 16);
            this.otherInformationLbl.Name = "otherInformationLbl";
            this.otherInformationLbl.Size = new System.Drawing.Size(350, 154);
            this.otherInformationLbl.TabIndex = 4;
            this.otherInformationLbl.Text = "Other Information";
            // 
            // statusCodeLbl
            // 
            this.statusCodeLbl.AutoSize = true;
            this.statusCodeLbl.Location = new System.Drawing.Point(49, 20);
            this.statusCodeLbl.Name = "statusCodeLbl";
            this.statusCodeLbl.Size = new System.Drawing.Size(43, 13);
            this.statusCodeLbl.TabIndex = 3;
            this.statusCodeLbl.Text = "200 OK";
            // 
            // httpVersionLbl
            // 
            this.httpVersionLbl.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.httpVersionLbl.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.httpVersionLbl.Location = new System.Drawing.Point(49, 7);
            this.httpVersionLbl.Name = "httpVersionLbl";
            this.httpVersionLbl.Size = new System.Drawing.Size(161, 13);
            this.httpVersionLbl.TabIndex = 2;
            this.httpVersionLbl.Text = "1.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Status";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "HTTP v";
            // 
            // rawOutTab
            // 
            this.rawOutTab.Controls.Add(this.OutputTxt);
            this.rawOutTab.Location = new System.Drawing.Point(4, 22);
            this.rawOutTab.Name = "rawOutTab";
            this.rawOutTab.Padding = new System.Windows.Forms.Padding(3);
            this.rawOutTab.Size = new System.Drawing.Size(402, 219);
            this.rawOutTab.TabIndex = 0;
            this.rawOutTab.Text = "Raw";
            this.rawOutTab.UseVisualStyleBackColor = true;
            // 
            // OutputTxt
            // 
            this.OutputTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputTxt.Location = new System.Drawing.Point(3, 3);
            this.OutputTxt.Multiline = true;
            this.OutputTxt.Name = "OutputTxt";
            this.OutputTxt.ReadOnly = true;
            this.OutputTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputTxt.Size = new System.Drawing.Size(396, 213);
            this.OutputTxt.TabIndex = 1;
            this.OutputTxt.WordWrap = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.rawOutTab);
            this.tabControl1.Controls.Add(this.summaryOutTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 16);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(410, 245);
            this.tabControl1.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.hostLbl);
            this.groupBox2.Controls.Add(this.InputTxt);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.errorLbl);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(386, 261);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Request";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tabControl1);
            this.groupBox3.Location = new System.Drawing.Point(404, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(416, 264);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Response";
            // 
            // Http
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 351);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.GoBtn);
            this.Name = "Http";
            this.Text = "HTTP Headers";
            this.summaryOutTab.ResumeLayout(false);
            this.summaryOutTab.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.rawOutTab.ResumeLayout(false);
            this.rawOutTab.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox InputTxt;
        private System.Windows.Forms.Button GoBtn;
        private System.Windows.Forms.Label errorLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label hostLbl;
        private System.Windows.Forms.TabPage summaryOutTab;
        private System.Windows.Forms.TabPage rawOutTab;
        private System.Windows.Forms.TextBox OutputTxt;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label statusCodeLbl;
        private System.Windows.Forms.Label httpVersionLbl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label otherInformationLbl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

