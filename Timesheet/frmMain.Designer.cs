namespace AutoRegisterTimesheet
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.dgv = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeStamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Username = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.totalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.totalsWeeklyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.totalsMonthlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblYesterdayRD = new System.Windows.Forms.Label();
            this.lblYesterdayTotal = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTodayRemote = new System.Windows.Forms.Label();
            this.lblTodayDesktop = new System.Windows.Forms.Label();
            this.lblTotalRemote = new System.Windows.Forms.Label();
            this.lblTotalDesktop = new System.Windows.Forms.Label();
            this.lblTotalToday = new System.Windows.Forms.Label();
            this.lblTotalWeek = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timerUpdateStatistics = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Timesheet";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // dgv
            // 
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.TimeStamp,
            this.Username,
            this.Action});
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(0, 27);
            this.dgv.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(760, 395);
            this.dgv.TabIndex = 0;
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // TimeStamp
            // 
            this.TimeStamp.HeaderText = "TimeStamp";
            this.TimeStamp.Name = "TimeStamp";
            this.TimeStamp.ReadOnly = true;
            // 
            // Username
            // 
            this.Username.HeaderText = "Username";
            this.Username.Name = "Username";
            this.Username.ReadOnly = true;
            // 
            // Action
            // 
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(960, 27);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.exitToolStripMenuItem,
            this.saveCSVToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // saveCSVToolStripMenuItem
            // 
            this.saveCSVToolStripMenuItem.Name = "saveCSVToolStripMenuItem";
            this.saveCSVToolStripMenuItem.Size = new System.Drawing.Size(135, 24);
            this.saveCSVToolStripMenuItem.Text = "Save CSV";
            this.saveCSVToolStripMenuItem.Visible = false;
            this.saveCSVToolStripMenuItem.Click += new System.EventHandler(this.saveCSVToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.totalsToolStripMenuItem,
            this.totalsWeeklyToolStripMenuItem,
            this.totalsMonthlyToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(50, 23);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Checked = true;
            this.logToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // totalsToolStripMenuItem
            // 
            this.totalsToolStripMenuItem.Name = "totalsToolStripMenuItem";
            this.totalsToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.totalsToolStripMenuItem.Text = "Totals - Daily";
            this.totalsToolStripMenuItem.Click += new System.EventHandler(this.totalsToolStripMenuItem_Click);
            // 
            // totalsWeeklyToolStripMenuItem
            // 
            this.totalsWeeklyToolStripMenuItem.Name = "totalsWeeklyToolStripMenuItem";
            this.totalsWeeklyToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.totalsWeeklyToolStripMenuItem.Text = "Totals - Weekly";
            this.totalsWeeklyToolStripMenuItem.Click += new System.EventHandler(this.totalsWeeklyToolStripMenuItem_Click);
            // 
            // totalsMonthlyToolStripMenuItem
            // 
            this.totalsMonthlyToolStripMenuItem.Name = "totalsMonthlyToolStripMenuItem";
            this.totalsMonthlyToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.totalsMonthlyToolStripMenuItem.Text = "Totals - Monthly";
            this.totalsMonthlyToolStripMenuItem.Click += new System.EventHandler(this.totalsMonthlyToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblYesterdayRD);
            this.panel1.Controls.Add(this.lblYesterdayTotal);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.lblTodayRemote);
            this.panel1.Controls.Add(this.lblTodayDesktop);
            this.panel1.Controls.Add(this.lblTotalRemote);
            this.panel1.Controls.Add(this.lblTotalDesktop);
            this.panel1.Controls.Add(this.lblTotalToday);
            this.panel1.Controls.Add(this.lblTotalWeek);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(760, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 395);
            this.panel1.TabIndex = 8;
            // 
            // lblYesterdayRD
            // 
            this.lblYesterdayRD.AutoSize = true;
            this.lblYesterdayRD.Location = new System.Drawing.Point(117, 218);
            this.lblYesterdayRD.Name = "lblYesterdayRD";
            this.lblYesterdayRD.Size = new System.Drawing.Size(41, 15);
            this.lblYesterdayRD.TabIndex = 22;
            this.lblYesterdayRD.Text = "label9";
            // 
            // lblYesterdayTotal
            // 
            this.lblYesterdayTotal.AutoSize = true;
            this.lblYesterdayTotal.Location = new System.Drawing.Point(117, 190);
            this.lblYesterdayTotal.Name = "lblYesterdayTotal";
            this.lblYesterdayTotal.Size = new System.Drawing.Size(41, 15);
            this.lblYesterdayTotal.TabIndex = 21;
            this.lblYesterdayTotal.Text = "label9";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 218);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 15);
            this.label8.TabIndex = 20;
            this.label8.Text = "Yesterday R+D";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 15);
            this.label7.TabIndex = 19;
            this.label7.Text = "Yesterday Total";
            // 
            // lblTodayRemote
            // 
            this.lblTodayRemote.AutoSize = true;
            this.lblTodayRemote.Location = new System.Drawing.Point(110, 98);
            this.lblTodayRemote.Name = "lblTodayRemote";
            this.lblTodayRemote.Size = new System.Drawing.Size(41, 15);
            this.lblTodayRemote.TabIndex = 18;
            this.lblTodayRemote.Text = "label7";
            // 
            // lblTodayDesktop
            // 
            this.lblTodayDesktop.AutoSize = true;
            this.lblTodayDesktop.Location = new System.Drawing.Point(110, 69);
            this.lblTodayDesktop.Name = "lblTodayDesktop";
            this.lblTodayDesktop.Size = new System.Drawing.Size(41, 15);
            this.lblTodayDesktop.TabIndex = 17;
            this.lblTodayDesktop.Text = "label7";
            // 
            // lblTotalRemote
            // 
            this.lblTotalRemote.AutoSize = true;
            this.lblTotalRemote.Location = new System.Drawing.Point(117, 282);
            this.lblTotalRemote.Name = "lblTotalRemote";
            this.lblTotalRemote.Size = new System.Drawing.Size(41, 15);
            this.lblTotalRemote.TabIndex = 16;
            this.lblTotalRemote.Text = "label7";
            // 
            // lblTotalDesktop
            // 
            this.lblTotalDesktop.AutoSize = true;
            this.lblTotalDesktop.Location = new System.Drawing.Point(117, 258);
            this.lblTotalDesktop.Name = "lblTotalDesktop";
            this.lblTotalDesktop.Size = new System.Drawing.Size(41, 15);
            this.lblTotalDesktop.TabIndex = 15;
            this.lblTotalDesktop.Text = "label7";
            // 
            // lblTotalToday
            // 
            this.lblTotalToday.AutoSize = true;
            this.lblTotalToday.Location = new System.Drawing.Point(110, 44);
            this.lblTotalToday.Name = "lblTotalToday";
            this.lblTotalToday.Size = new System.Drawing.Size(41, 15);
            this.lblTotalToday.TabIndex = 14;
            this.lblTotalToday.Text = "label7";
            // 
            // lblTotalWeek
            // 
            this.lblTotalWeek.AutoSize = true;
            this.lblTotalWeek.Location = new System.Drawing.Point(110, 19);
            this.lblTotalWeek.Name = "lblTotalWeek";
            this.lblTotalWeek.Size = new System.Drawing.Size(41, 15);
            this.lblTotalWeek.TabIndex = 13;
            this.lblTotalWeek.Text = "label7";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "Today Remote";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "Today Desktop";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 282);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Total Remote";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 258);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Total Desktop";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Total Today";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "Total Week";
            // 
            // timerUpdateStatistics
            // 
            this.timerUpdateStatistics.Enabled = true;
            this.timerUpdateStatistics.Interval = 1000;
            this.timerUpdateStatistics.Tick += new System.EventHandler(this.timerUpdateStatistics_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 422);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "Time sheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeStamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Username;
        private System.Windows.Forms.DataGridViewTextBoxColumn Action;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTodayRemote;
        private System.Windows.Forms.Label lblTodayDesktop;
        private System.Windows.Forms.Label lblTotalRemote;
        private System.Windows.Forms.Label lblTotalDesktop;
        private System.Windows.Forms.Label lblTotalToday;
        private System.Windows.Forms.Label lblTotalWeek;
        private System.Windows.Forms.Label lblYesterdayRD;
        private System.Windows.Forms.Label lblYesterdayTotal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripMenuItem saveCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem totalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem totalsWeeklyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem totalsMonthlyToolStripMenuItem;
        private System.Windows.Forms.Timer timerUpdateStatistics;
    }
}

