﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaCollectionModel.cs" company="The YANFOE Project">
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

namespace YANFOE.Models.GeneralModels
{
    using System.ComponentModel;

    using YANFOE.Models.GeneralModels.AssociatedFiles;

    /// <summary>
    /// The media collection factory.
    /// </summary>
    public sealed class MediaCollection
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaCollection"/> class.
        /// </summary>
        public MediaCollection()
        {
            this.File = new BindingList<MediaModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets File.
        /// </summary>
        public BindingList<MediaModel> File { get; set; }

        #endregion
    }
}