namespace AutoRegisterTimesheet
{
    partial class frmSettings
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
            this.cmbFirstDay = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDailyHours = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbAlertDailyHours = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "First Day of Week";
            // 
            // cmbFirstDay
            // 
            this.cmbFirstDay.FormattingEnabled = true;
            this.cmbFirstDay.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
            this.cmbFirstDay.Location = new System.Drawing.Point(121, 13);
            this.cmbFirstDay.Name = "cmbFirstDay";
            this.cmbFirstDay.Size = new System.Drawing.Size(121, 21);
            this.cmbFirstDay.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Hours Per Day";
            this.label2.Visible = false;
            // 
            // txtDailyHours
            // 
            this.txtDailyHours.Location = new System.Drawing.Point(121, 41);
            this.txtDailyHours.Name = "txtDailyHours";
            this.txtDailyHours.Size = new System.Drawing.Size(121, 20);
            this.txtDailyHours.TabIndex = 3;
            this.txtDailyHours.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(288, 104);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(207, 104);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbAlertDailyHours
            // 
            this.cbAlertDailyHours.AutoSize = true;
            this.cbAlertDailyHours.Location = new System.Drawing.Point(16, 72);
            this.cbAlertDailyHours.Name = "cbAlertDailyHours";
            this.cbAlertDailyHours.Size = new System.Drawing.Size(190, 19);
            this.cbAlertDailyHours.TabIndex = 6;
            this.cbAlertDailyHours.Text = "Alert when daily hours are met";
            this.cbAlertDailyHours.UseVisualStyleBackColor = true;
            this.cbAlertDailyHours.Visible = false;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 137);
            this.Controls.Add(this.cbAlertDailyHours);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDailyHours);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbFirstDay);
            this.Controls.Add(this.label1);
            this.Name = "frmSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbFirstDay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDailyHours;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbAlertDailyHours;
    }
}