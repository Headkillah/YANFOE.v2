﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScanSeason.cs" company="The YANFOE Project">
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

namespace YANFOE.Models.TvModels.Scan
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The scan season.
    /// </summary>
    [Serializable]
    public class ScanSeason
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanSeason"/> class.
        /// </summary>
        public ScanSeason()
        {
            this.Episodes = new SortedDictionary<int, ScanEpisode>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Episodes.
        /// </summary>
        public SortedDictionary<int, ScanEpisode> Episodes { get; set; }

        #endregion
    }
}