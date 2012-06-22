using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace AutoRegisterTimesheet
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// Timesheet instance for handling everything timesheet
        /// </summary>
        private Timesheet timesheet = new Timesheet("Log.txt");

        /// <summary>
        /// Allow closing the application
        /// </summary>
        private bool AllowClose = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings sett = new frmSettings();
            sett.ShowDialog();
            LoadSettings();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllowClose = true;

            //cleanup
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;

            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //Load Settings
            LoadSettings();

            //load from log
            timesheet.LoadFromLog();

            //initial display in grid
            logToolStripMenuItem.Checked = false;
            logToolStripMenuItem_Click(null, null);

            //event handler for session switching events
            Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

            //add to log that we started
            AppendToLog(timesheet.RecordNumber++, DateTime.Now, GetLoggedInUsername(), "Started");
        }

        private static string GetLoggedInUsername()
        {
            return WindowsIdentity.GetCurrent().Name;
        }

        private void LoadSettings()
        {
            timesheet.FirstDayOfWeek = Properties.Settings.Default.FirstDayOfWeek;
            timesheet.MinimumHoursPerDay = Properties.Settings.Default.HoursPerDay;
            timesheet.AlertDailyHoursMet = Properties.Settings.Default.AlertDailyHoursMet;
        }

        private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            AppendToLog(timesheet.RecordNumber++, DateTime.Now, GetLoggedInUsername(), e.Reason.ToString());
        }

        private void AppendToLog(int id, DateTime timestamp, string username, string switchname)
        {
            if (logToolStripMenuItem.Checked == true)
                dgv.Rows.Add(id,timestamp,username,switchname);

            timesheet.AppendToLog(id, timestamp, username, switchname);
        }
        
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppendToLog(timesheet.RecordNumber++, DateTime.Now, GetLoggedInUsername(), "Closing");

            if (AllowClose == false)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
     
        private string FormatTimespan(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}",ts.TotalHours,ts.Minutes,ts.Seconds);
        }

        private void timerUpdateStatistics_Tick(object sender, EventArgs e)
        {
            MethodInvoker mi = new MethodInvoker(timesheet.CalculateStatisticsResults);
            mi.BeginInvoke(null, null);

            CalculateStatistics();
        }

        private void CalculateStatistics()
        {
            var username = GetLoggedInUsername();
            var startOfWeek = timesheet.FirstDateOfWeek(DateTime.Now);
            var startOfDay = DateTime.Now.Date;
            var yesterday = DateTime.Now.Date - TimeSpan.FromDays(1);

            var totalhours = timesheet.StatisticsResults;


            lblTodayDesktop.Text = FormatTimespan( (from ll in totalhours where ll.Date == startOfDay select ll.TotalHours - ll.TotalRemote).FirstOrDefault());
            lblTodayRemote.Text = FormatTimespan((from ll in totalhours where ll.Date == startOfDay select ll.TotalRemote).FirstOrDefault());

            lblTotalDesktop.Text = FormatTimespan(TimeSpan.FromHours ((from ll in totalhours select ll.TotalHours.TotalHours - ll.TotalRemote.TotalHours).Sum()));
            lblTotalRemote.Text = FormatTimespan(TimeSpan.FromHours((from ll in totalhours select ll.TotalRemote.TotalHours).Sum()));
            lblTotalToday.Text = FormatTimespan((from ll in totalhours where ll.Date == startOfDay select ll.TotalHours).FirstOrDefault());
            lblTotalWeek.Text = FormatTimespan(TimeSpan.FromHours((from ll in totalhours where ll.Date >= startOfWeek && ll.Date <= DateTime.Now.Date select ll.TotalHours.TotalHours).Sum()));


            lblYesterdayRD.Text = FormatTimespan((from ll in totalhours where ll.Date == yesterday select ll.TotalHours - ll.TotalLocked).FirstOrDefault());
            lblYesterdayTotal.Text = FormatTimespan((from ll in totalhours where ll.Date == yesterday select ll.TotalHours).FirstOrDefault());
        }


        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logToolStripMenuItem.Checked == true)
                return;

            logToolStripMenuItem.Checked = true;
            totalsWeeklyToolStripMenuItem.Checked = false;
            totalsMonthlyToolStripMenuItem.Checked = false;
            totalsToolStripMenuItem.Checked = false;

            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("Id", "Id");
            dgv.Columns.Add("Timestamp", "Timestamp");
            dgv.Columns.Add("Username", "Username");
            dgv.Columns.Add("Action", "Action");

            int i = 0;
            foreach (var ll in timesheet.LogLines.OrderBy(o=>o.Timestamp))
                dgv.Rows.Add(++i, ll.Timestamp, ll.Username, ll.Event);
        }

        private void totalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (totalsToolStripMenuItem.Checked == true)
                return;
            
            totalsToolStripMenuItem.Checked = true;
            logToolStripMenuItem.Checked = false;
            totalsWeeklyToolStripMenuItem.Checked = false;
            totalsMonthlyToolStripMenuItem.Checked = false;


            dgv.Rows.Clear();
            dgv.Columns.Clear();

            var statistics = timesheet.StatisticsResults;
            dgv.Columns.Add("Date", "Date");
            dgv.Columns.Add("TotalHours", "Total Hours");
            dgv.Columns.Add("TotalRemote", "Total Remote");
            dgv.Columns.Add("TotalAway", "Total Away");

            foreach (var stat in statistics.OrderByDescending(i => i.Date))
                dgv.Rows.Add(stat.Date.ToShortDateString(),FormatTimespan( stat.TotalHours), FormatTimespan(stat.TotalRemote),FormatTimespan( stat.TotalLocked));
            
        }

        private void totalsWeeklyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (totalsWeeklyToolStripMenuItem.Checked == true)
                return;

            totalsWeeklyToolStripMenuItem.Checked = true;
            totalsToolStripMenuItem.Checked = false;
            logToolStripMenuItem.Checked = false;
            totalsMonthlyToolStripMenuItem.Checked = false;


            dgv.Rows.Clear();
            dgv.Columns.Clear();

            var statistics = timesheet.StatisticsResults;

            var statsweekly = (from s in statistics.AsParallel()
                               group s by new { Year = s.Date.Year, Week = timesheet.GetWeekNumber(s.Date) } into sg
                              select new
                                  {
                                      Year = sg.Key.Year,
                                       Week = sg.Key.Week,
                                       TotalHours = TimeSpan.FromHours( sg.Sum(i=>i.TotalHours.Hours)),
                                       TotalRemote = TimeSpan.FromHours( sg.Sum(i=>i.TotalRemote.Hours)),
                                       TotalLocked = TimeSpan.FromHours(sg.Sum(i=>i.TotalLocked.Hours))
                                  }).ToList();

            dgv.Columns.Add("Week", "Week");
            dgv.Columns.Add("Starts At", "StartsAt");
            dgv.Columns.Add("TotalHours", "Total Hours");
            dgv.Columns.Add("TotalRemote", "Total Remote");
            dgv.Columns.Add("TotalAway", "Total Away");

            foreach (var stat in statsweekly.OrderByDescending(i=>i.Year).ThenByDescending(i => i.Week))
                dgv.Rows.Add(stat.Year + "\\" + stat.Week,timesheet.GetFirstDateOfWeek(stat.Year,stat.Week), FormatTimespan(stat.TotalHours), FormatTimespan(stat.TotalRemote), FormatTimespan(stat.TotalLocked));
        }

       

        private void totalsMonthlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (totalsMonthlyToolStripMenuItem.Checked == true)
                return;

            totalsMonthlyToolStripMenuItem.Checked = true;
            totalsWeeklyToolStripMenuItem.Checked = false;
            totalsToolStripMenuItem.Checked = false;
            logToolStripMenuItem.Checked = false;
            


            dgv.Rows.Clear();
            dgv.Columns.Clear();

            var statistics = timesheet.StatisticsResults;

            var statsweekly = (from s in statistics.AsParallel()
                               group s by new { Year = s.Date.Year,Month = s.Date.Month } into sg
                               select new
                               {
                                   Year = sg.Key.Year,
                                   Month = sg.Key.Month,
                                   TotalHours = TimeSpan.FromHours(sg.Sum(i => i.TotalHours.Hours)),
                                   TotalRemote = TimeSpan.FromHours(sg.Sum(i => i.TotalRemote.Hours)),
                                   TotalLocked = TimeSpan.FromHours(sg.Sum(i => i.TotalLocked.Hours))
                               }).ToList();

            dgv.Columns.Add("Month", "Month");
            dgv.Columns.Add("TotalHours", "Total Hours");
            dgv.Columns.Add("TotalRemote", "Total Remote");
            dgv.Columns.Add("TotalAway", "Total Away");

            foreach (var stat in statsweekly.OrderByDescending(i => i.Year).ThenByDescending(i => i.Month))
            {
                dgv.Rows.Add(stat.Year + "\\" + stat.Month, FormatTimespan(stat.TotalHours), FormatTimespan(stat.TotalRemote), FormatTimespan(stat.TotalLocked));
            }
        }


        private void saveCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

       

    }
}
