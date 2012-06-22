using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoRegisterTimesheet
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            cmbFirstDay.SelectedItem = Properties.Settings.Default.FirstDayOfWeek.ToString();
            txtDailyHours.Text = Properties.Settings.Default.HoursPerDay.ToString();
            cbAlertDailyHours.Checked = Properties.Settings.Default.AlertDailyHoursMet;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            Properties.Settings.Default.FirstDayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), cmbFirstDay.SelectedItem as string);
            int hoursperday = 0;
            if (!int.TryParse(txtDailyHours.Text, out hoursperday))
            {
                MessageBox.Show("Hours per day must be selected");
                return;
            }

            Properties.Settings.Default.HoursPerDay = hoursperday;
            Properties.Settings.Default.AlertDailyHoursMet = cbAlertDailyHours.Checked;

            Properties.Settings.Default.Save();

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
