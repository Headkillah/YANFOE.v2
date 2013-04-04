﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsUserControl.cs" company="The YANFOE Project">
//   Copyright 2011 The YANFOE Project
// </copyright>
// <license>
//   This software is licensed under a Creative Commons License
//   Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) 
//   http://creativecommons.org/licenses/by-nc-sa/3.0/
//   See this page: http://www.yanfoe.com/license
//   For any reuse or distribution, you must make clear to others the 
//   license terms of this work.  
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace YANFOE.UI.UserControls.LogControls
{
    using System.Windows.Forms;

    using YANFOE.InternalApps.Logs.Enums;

    public partial class LogsUserControl : DevExpress.XtraEditors.XtraUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogsUserControl"/> class.
        /// </summary>
        public LogsUserControl()
        {
            InitializeComponent();

            grdLog.DataSource = InternalApps.Logs.Log.GetInternalLogger(LoggerName.GeneralLog);

            chbEnableLog.DataBindings.Add(
                "Checked", Settings.Get.LogSettings, "EnableLog", true, DataSourceUpdateMode.OnPropertyChanged);
            int index = 1;
            switch (InternalApps.Logs.Log.logTreshold)
            {
                case BitFactory.Logging.LogSeverity.Debug:
                    index = 1;
                    break;
                case BitFactory.Logging.LogSeverity.Info:
                    index = 2;
                    break;
                case BitFactory.Logging.LogSeverity.Status:
                    index = 3;
                    break;
                case BitFactory.Logging.LogSeverity.Warning:
                    index = 4;
                    break;
                case BitFactory.Logging.LogSeverity.Error:
                    index = 5;
                    break;
                case BitFactory.Logging.LogSeverity.Critical:
                    index = 6;
                    break;
                case BitFactory.Logging.LogSeverity.Fatal:
                    index = 7;
                    break;
            }
            radioGroup1.SelectedIndex = index - 1;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chbEnableLog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chbEnableLog_CheckedChanged(object sender, System.EventArgs e)
        {
            this.chbEnableLog.Text = this.chbEnableLog.Checked ? "Disable Log" : "Enable Log";
        }

        /// <summary>
        /// Handles the Click event of the btnClearLog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnClearLog_Click(object sender, System.EventArgs e)
        {
            InternalApps.Logs.Log.ClearInternalLogger(LoggerName.GeneralLog);
        }

        private void radioGroup1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var logTreshold = (BitFactory.Logging.LogSeverity)System.Enum.ToObject(typeof(BitFactory.Logging.LogSeverity), radioGroup1.SelectedIndex + 1);
            InternalApps.Logs.Log.logTreshold = logTreshold;
        }
    }
}
