﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="The YANFOE Project" file="OutFactory.cs">
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
// <summary>
//   The out factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace YANFOE.Factories.InOut
{
    #region Required Namespaces

    using System;
    using System.ComponentModel;

    using BitFactory.Logging;

    using YANFOE.Factories.InOut.Enum;
    using YANFOE.Factories.InOut.Model;
    using YANFOE.Factories.UI;
    using YANFOE.InternalApps.Logs;
    using YANFOE.IO;
    using YANFOE.Models.IOModels;
    using YANFOE.Models.MovieModels;
    using YANFOE.Models.TvModels.Show;
    using YANFOE.Settings;
    using YANFOE.Tools.Enums;

    #endregion

    /// <summary>
    ///   The out factory.
    /// </summary>
    public static class OutFactory
    {
        #region Static Fields

        /// <summary>
        ///   The XBMC handler object.
        /// </summary>
        private static readonly XBMC Xbmc = new XBMC();

        // private static readonly WndSavingDialog dialog = new WndSavingDialog();

        /// <summary>
        ///   The Saving dialog.
        /// </summary>
        /// <summary>
        ///   The YAMJ handler object.
        /// </summary>
        private static readonly YAMJ Yamj = new YAMJ();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="OutFactory" /> class.
        /// </summary>
        static OutFactory()
        {
            ProgressModel = new SaveProgressModel();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="OutFactory" /> is cancel.
        /// </summary>
        /// <value> <c>true</c> if cancel; otherwise, <c>false</c> . </value>
        public static bool Cancel { get; set; }

        /// <summary>
        ///   Gets or sets the episode max.
        /// </summary>
        public static int EpisodeMax { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="OutFactory" /> is finished.
        /// </summary>
        /// <value> <c>true</c> if finished; otherwise, <c>false</c> . </value>
        public static bool Finished { get; set; }

        /// <summary>
        ///   Gets or sets the progress model.
        /// </summary>
        /// <value> The progress model. </value>
        public static SaveProgressModel ProgressModel { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether [refresh grids].
        /// </summary>
        /// <value> <c>true</c> if [refresh grids]; otherwise, <c>false</c> . </value>
        public static bool RefreshGrids { get; set; }

        /// <summary>
        ///   Gets or sets the season max.
        /// </summary>
        public static int SeasonMax { get; set; }

        /// <summary>
        ///   Gets or sets the series max.
        /// </summary>
        public static int SeriesMax { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The load movie.
        /// </summary>
        /// <param name="movie">
        /// The movie. 
        /// </param>
        /// <returns>
        /// Always returns true 
        /// </returns>
        public static bool LoadMovie(MovieModel movie)
        {
            try
            {
                if (Get.InOutCollection.IoType == NFOType.YAMJ)
                {
                    Yamj.LoadMovie(movie);
                }
                else if (Get.InOutCollection.IoType == NFOType.XBMC)
                {
                    Xbmc.LoadMovie(movie);
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "LoadMovie", exception.Message);
                return false;
            }
        }

        /// <summary>
        /// Load Series
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Process succeeded. 
        /// </returns>
        public static bool LoadSeries(Series series)
        {
            try
            {
                if (Get.InOutCollection.IoType == NFOType.YAMJ)
                {
                    Yamj.LoadSeries(series);
                }
                else if (Get.InOutCollection.IoType == NFOType.XBMC)
                {
                    Xbmc.LoadSeries(series);
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "LoadSeries", exception.Message);
                return false;
            }
        }

        /// <summary>
        /// The save all selected episode details.
        /// </summary>
        /// <param name="type">
        /// The EpisodeIOType type. 
        /// </param>
        /// <returns>
        /// Process succeeded 
        /// </returns>
        public static bool SaveAllSelectedEpisodeDetails(EpisodeIOType type = EpisodeIOType.All)
        {
            try
            {
                foreach (var episode in TVDBFactory.Instance.CurrentSelectedEpisode)
                {
                    SaveSpecificEpisode(episode, type);
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveAllSelectedEpisodeDetails", exception.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The save all selected season details.
        /// </summary>
        /// <param name="type">
        /// The SeasonIOType type. 
        /// </param>
        /// <returns>
        /// Process Succeeded. 
        /// </returns>
        public static bool SaveAllSelectedSeasonDetails(SeasonIOType type = SeasonIOType.All)
        {
            try
            {
                foreach (Season season in TVDBFactory.Instance.CurrentSelectedSeason)
                {
                    SaveSpecificSeason(season, type);
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveAllSelectedSeasonDetails", exception.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The save all selected series details.
        /// </summary>
        /// <param name="type">
        /// The SeriesIOType type. 
        /// </param>
        /// <returns>
        /// Process succeeded. 
        /// </returns>
        public static bool SaveAllSelectedSeriesDetails(SeriesIOType type = SeriesIOType.All)
        {
            try
            {
                foreach (var series in TVDBFactory.Instance.CurrentSelectedSeries)
                {
                    SaveSpecificSeries(series, type);
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveAllSelectedSeriesDetails", exception.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The save movie.
        /// </summary>
        /// <param name="type">
        /// The MovieIOType type. 
        /// </param>
        /// <returns>
        /// Process succeeded status. 
        /// </returns>
        public static bool SaveMovie(MovieIOType type = MovieIOType.All)
        {
            try
            {
                MovieSaveSettings movieSaveSettings = Get.InOutCollection.MovieSaveSettings[Get.InOutCollection.IoType];

                movieSaveSettings.IoType = type;

                foreach (MovieModel movie in MovieDBFactory.Instance.MultiSelectedMovies)
                {
                    if (Get.InOutCollection.IoType == NFOType.YAMJ)
                    {
                        Yamj.SaveMovie(movie);
                    }
                    else if (Get.InOutCollection.IoType == NFOType.XBMC)
                    {
                        Xbmc.SaveMovie(movie);
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveMovie", exception.Message);
                return false;
            }
        }

        /// <summary>
        /// The save recursive selected series details.
        /// </summary>
        /// <param name="type">
        /// TVIoType type. 
        /// </param>
        public static void SaveRecursiveSelectedSeriesDetails(TVIoType type = TVIoType.All)
        {
            Cancel = false;

            var bgwSaveRecursiveSelectedSeriesDetails = new BackgroundWorker();

            bgwSaveRecursiveSelectedSeriesDetails.DoWork += BgwSaveRecursiveSelectedSeriesDetails_DoWork;
            bgwSaveRecursiveSelectedSeriesDetails.RunWorkerCompleted +=
                BgwSaveRecursiveSelectedSeriesDetails_RunWorkerCompleted;

            bgwSaveRecursiveSelectedSeriesDetails.RunWorkerAsync();

            SeriesMax = TVDBFactory.Instance.CurrentSelectedSeries.Count;

            SeasonMax = 0;

            EpisodeMax = 0;

            foreach (Series series in TVDBFactory.Instance.CurrentSelectedSeries)
            {
                SeasonMax += series.Seasons.Count;

                foreach (var season in series.Seasons)
                {
                    EpisodeMax += season.Episodes.Count;
                }
            }
        }

        /// <summary>
        /// The save specific episode.
        /// </summary>
        /// <param name="episode">
        /// The episode. 
        /// </param>
        /// <param name="type">
        /// The EpisodeIOType type. 
        /// </param>
        /// <returns>
        /// Process succeeded 
        /// </returns>
        public static bool SaveSpecificEpisode(Episode episode, EpisodeIOType type = EpisodeIOType.All)
        {
            try
            {
                if (Get.InOutCollection.IoType == NFOType.YAMJ)
                {
                    Yamj.SaveEpisode(episode, type);
                }
                else if (Get.InOutCollection.IoType == NFOType.XBMC)
                {
                    Xbmc.SaveEpisode(episode, type);
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveSpecificEpisode", exception.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The save specific season.
        /// </summary>
        /// <param name="season">
        /// The season. 
        /// </param>
        /// <param name="type">
        /// The SeasonIOType type. 
        /// </param>
        /// <returns>
        /// Process Succeeded 
        /// </returns>
        public static bool SaveSpecificSeason(Season season, SeasonIOType type = SeasonIOType.All)
        {
            try
            {
                if (Get.InOutCollection.IoType == NFOType.YAMJ)
                {
                    Yamj.SaveSeason(season, type);
                }
                else if (Get.InOutCollection.IoType == NFOType.XBMC)
                {
                    Xbmc.SaveSeason(season, type);
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveSpecificSeason", exception.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the specific series.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <param name="type">
        /// The SeriesIOType type. 
        /// </param>
        /// <returns>
        /// Process succeeded 
        /// </returns>
        public static bool SaveSpecificSeries(Series series, SeriesIOType type = SeriesIOType.All)
        {
            try
            {
                if (Get.InOutCollection.IoType == NFOType.YAMJ)
                {
                    Yamj.SaveSeries(series, type);
                }
                else if (Get.InOutCollection.IoType == NFOType.XBMC)
                {
                    Xbmc.SaveSeries(series, type);
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveSpecificSeries", exception.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the DoWork event of the Bgw_SaveRecursiveSelectedSeriesDetails control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data. 
        /// </param>
        private static void BgwSaveRecursiveSelectedSeriesDetails_DoWork(object sender, DoWorkEventArgs e)
        {
            ProgressModel.EnableSeries = true;
            ProgressModel.EnableSeason = true;
            ProgressModel.EnableEpisode = true;

            ProgressModel.SeriesCurrent = 1;
            ProgressModel.SeasonCurrent = 1;
            ProgressModel.EpisodeCurrent = 1;

            try
            {
                foreach (var series in TVDBFactory.Instance.CurrentSelectedSeries)
                {
                    ProgressModel.SeriesText = series.SeriesName;

                    SaveSpecificSeries(series);

                    Log.WriteToLog(LogSeverity.Info, 0, "Saving " + series.SeriesName, string.Empty);

                    foreach (var season in series.Seasons)
                    {
                        Log.WriteToLog(
                            LogSeverity.Info, 
                            0, 
                            "Saving " + series.SeriesName + ", Season " + season.SeasonNumber, 
                            string.Empty);

                        ProgressModel.SeasonText = "Season " + season.SeasonNumber;

                        SaveSpecificSeason(season);

                        foreach (Episode episode in season.Episodes)
                        {
                            ProgressModel.EpisodeText = episode.EpisodeName;

                            SaveSpecificEpisode(episode);

                            ProgressModel.EpisodeCurrent++;

                            Windows7UIFactory.SetProgressValue(ProgressModel.EpisodeCurrent);

                            if (Cancel)
                            {
                                Cancel = false;
                                return;
                            }
                        }

                        ProgressModel.SeasonCurrent++;
                    }

                    ProgressModel.SeriesCurrent++;
                }

                ProgressModel.SeriesText = string.Empty;
                ProgressModel.SeasonText = string.Empty;
                ProgressModel.EpisodeText = string.Empty;
            }
            catch (Exception exception)
            {
                Log.WriteToLog(LogSeverity.Error, 0, "SaveRecursiveSelectedSeriesDetails", exception.Message);
            }
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the bgwSaveRecursiveSelectedSeriesDetails control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data. 
        /// </param>
        private static void BgwSaveRecursiveSelectedSeriesDetails_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshGrids = true;
        }

        #endregion
    }
}