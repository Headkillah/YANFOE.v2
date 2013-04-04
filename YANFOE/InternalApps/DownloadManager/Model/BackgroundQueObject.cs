﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundQueObject.cs" company="The YANFOE Project">
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

namespace YANFOE.InternalApps.DownloadManager.Model
{
    using System;

    using YANFOE.Tools.Enums;

    /// <summary>
    /// The background que object.
    /// </summary>
    [Serializable]
    public class BackgroundQueObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets CacheSection.
        /// </summary>
        public Section CacheSection { get; set; }

        /// <summary>
        /// Gets or sets Type.
        /// </summary>
        public DownloadType Type { get; set; }

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        public string Url { get; set; }

        #endregion
    }
}