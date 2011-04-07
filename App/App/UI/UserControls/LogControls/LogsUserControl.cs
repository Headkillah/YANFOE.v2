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
    using YANFOE.InternalApps.Logs.Enums;

    public partial class LogsUserControl : DevExpress.XtraEditors.XtraUserControl
    {
        public LogsUserControl()
        {
            InitializeComponent();

            grdLog.DataSource = InternalApps.Logs.Log.GetInternalLogger(LoggerName.GeneralLog);
        }
    }
}
