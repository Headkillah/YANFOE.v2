﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppleTrailerResultsModel.cs" company="The YANFOE Project">
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

namespace YANFOE.Scrapers.Movie.Models.AppleTrailers
{
    using System;

    using DevExpress.XtraEditors.DXErrorProvider;

    using Newtonsoft.Json;

    using YANFOE.Models.MovieModels;
    using YANFOE.Tools.Models;

    using ErrorInfo = DevExpress.XtraEditors.DXErrorProvider.ErrorInfo;

    [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
    [Serializable]
    public class AppleTrailerResultsModel : ModelBase, IDXDataErrorInfo
    {

        #region Constants and Fields



        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieModel"/> class.
        /// </summary>
        public AppleTrailerResultsModel()
        {
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public string title { get; set; }

        public string releasedate { get; set; }

        public string studio { get; set; }

        public string poster { get; set; }

        public string moviesite { get; set; }

        public string location { get; set; }


        #endregion

        #region Public Methods

        #endregion

        #region Implemented Interfaces

        #region IDXDataErrorInfo

        /// <summary>
        /// When implemented by a class, this method returns information on an error associated with a business object.
        /// </summary>
        /// <param name="info">An <see cref="T:DevExpress.XtraEditors.DXErrorProvider.ErrorInfo"/> object that contains information on an error.</param>
        public void GetError(ErrorInfo info)
        {
        }

        /// <summary>
        /// When implemented by a class, this method returns information on an error associated with a specific business object's property.
        /// </summary>
        /// <param name="propertyName">A string that identifies the name of the property for which information on an error is to be returned.</param>
        /// <param name="info">An <see cref="T:DevExpress.XtraEditors.DXErrorProvider.ErrorInfo"/> object that contains information on an error.</param>
        public void GetPropertyError(string propertyName, ErrorInfo info)
        {
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}
