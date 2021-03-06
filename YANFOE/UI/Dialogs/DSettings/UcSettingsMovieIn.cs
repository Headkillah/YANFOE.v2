﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UcSettingsMovieIn.cs" company="The YANFOE Project">
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

namespace YANFOE.UI.Dialogs.DSettings
{
    public partial class UcSettingsMovieIn : DevExpress.XtraEditors.XtraUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UcSettingsMovieIn"/> class.
        /// </summary>
        public UcSettingsMovieIn()
        {
            InitializeComponent();

            txtMinimumFileSize.DataBindings.Add("Text", Settings.Get.InOutCollection, "MinimumMovieSize");
        }
    }
}
