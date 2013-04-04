﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TvUserControl.cs" company="The YANFOE Project">
//   Copyright 2011 The YANFOE Project
// </copyright>
// <summary>
//   Main TV user control
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YANFOE.UI.UserControls.TvControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using DevExpress.Data;
    using DevExpress.Utils;
    using DevExpress.XtraBars;
    using DevExpress.XtraBars.Ribbon;
    using DevExpress.XtraEditors;
    using DevExpress.XtraGrid.Views.Base;
    using DevExpress.XtraGrid.Views.Grid;
    using DevExpress.XtraGrid.Views.Grid.ViewInfo;

    using YANFOE.Factories;
    using YANFOE.Models.TvModels;
    using YANFOE.Models.TvModels.Show;
    using YANFOE.Properties;
    using YANFOE.Settings;

    /// <summary>
    /// Main TV user control
    /// </summary>
    public partial class TvUserControl : XtraUserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TvUserControl" /> class.
        /// </summary>
        public TvUserControl()
        {
            this.InitializeComponent();

            this.picSeriesBanner.HeaderTitle = "Banner";

            this.picSeriesFanart.HeaderTitle = "Fanart";

            this.picSeriesPoster.HeaderTitle = "Poster";

            this.picEpisodeFrame.HeaderTitle = "Episode Frame";

            this.grdTvTitleList.DataSource = TvDBFactory.MasterSeriesNameList;

            TvDBFactory.GalleryChanged += this.TvDBFactory_GalleryChanged;
            TvDBFactory.GeneratePictureGallery();

            TvDBFactory.CurrentSeriesChanged += this.TvDBFactory_CurrentSeriesChanged;
            TvDBFactory.CurrentSeasonChanged += this.TvDBFactory_CurrentSeasonChanged;
            TvDBFactory.MasterSeriesNameListChanged += this.TvDBFactory_MasterSeriesNameListChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles the CurrentSeriesChanged event of the TvDBFactory control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        public void TvDBFactory_CurrentSeriesChanged(object sender, EventArgs e)
        {
            this.UpdateSeasons();
            this.UpdateEpisodes();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemClick event of the Gallery control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs"/> instance containing the event data.
        /// </param>
        private void Gallery_ItemClick(object sender, GalleryItemClickEventArgs e)
        {
            this.gridViewTvTitleList.ClearSelection();

            var item =
                (from i in TvDBFactory.MasterSeriesNameList 
                 where i.SeriesGuid == e.Item.Tag.ToString()
                 select i).SingleOrDefault();

            var selectedSeries = TvDBFactory.MasterSeriesNameList.IndexOf(item);
            var handle = this.gridViewTvTitleList.GetRowHandle(selectedSeries);
            this.gridViewTvTitleList.FocusedRowHandle = handle;
            this.gridViewTvTitleList.SelectRow(handle);
            TvDBFactory.SetCurrentSeries(e.Item.Tag.ToString());
        }

        /// <summary>
        /// Handles the CurrentSeasonChanged event of the TvDBFactory control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void TvDBFactory_CurrentSeasonChanged(object sender, EventArgs e)
        {
            this.UpdateEpisodes();
        }

        /// <summary>
        /// Handles the GalleryChanged event of the TvDBFactory control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void TvDBFactory_GalleryChanged(object sender, EventArgs e)
        {
            tabBanner.Text = string.Format("Banner ({0})", this.galleryBanners.Gallery.Groups.Count);

            this.galleryBanners.Gallery.Groups.Clear();
            this.galleryBanners.Gallery.Groups.Add(TvDBFactory.GetGalleryGroup());
            this.galleryBanners.Gallery.ItemClick += this.Gallery_ItemClick;
        }

        /// <summary>
        /// Handles the MasterSeriesNameListChanged event of the TvDBFactory control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void TvDBFactory_MasterSeriesNameListChanged(object sender, EventArgs e)
        {
            tabTitle.Text = string.Format("Series Name ({0})", TvDBFactory.MasterSeriesNameList);
            this.gridViewTvTitleList.RefreshData();
            gridViewTvTitleList_FocusedRowChanged(null, null);
        }

        /// <summary>
        /// Updates the episodes datasource
        /// </summary>
        private void UpdateEpisodes()
        {
            this.grdEpisodes.DataSource = null;
            this.grdEpisodes.DataSource = TvDBFactory.GetCurrentEpisodeList();
        }

        /// <summary>
        /// Updates the seasons datasource
        /// </summary>
        private void UpdateSeasons()
        {
            this.grdSeasons.DataSource = null;
            this.grdSeasons.DataSource = TvDBFactory.GetCurrentSeasonsList;

            var season = this.gridViewSeasons.GetRow(this.gridViewSeasons.GetSelectedRows()[0]) as Season;

            TvDBFactory.SetCurrentSeason(season.Guid);
        }

        /// <summary>
        /// Handles the Click event of the btnAddCustomSeries control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void btnAddCustomSeries_Click(object sender, EventArgs e)
        {
            TvDBFactory.CreateCustomSeries();
        }

        /// <summary>
        /// Handles the DragDrop event of the grdEpisodes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.
        /// </param>
        private void grdEpisodes_DragDrop(object sender, DragEventArgs e)
        {
            GridHitInfo hitTest = this.gridViewEpisodes.CalcHitInfo(this.grdEpisodes.PointToClient(new Point(e.X, e.Y)));

            var episode = this.gridViewEpisodes.GetRow(hitTest.RowHandle) as Episode;

            if (hitTest.InRow)
            {
                var fileNameW = e.Data.GetData("FileNameW");

                if (fileNameW != null)
                {
                    var fileNames = (string[])fileNameW;
                    if (fileNames.Length == 1)
                    {
                        string fileName = fileNames[0];

                        if (
                            Get.InOutCollection.VideoExtentions.Contains(
                                Path.GetExtension(fileName.ToLower()).Replace(".", string.Empty)))
                        {
                            if (string.IsNullOrEmpty(episode.FilePath.PathAndFilename))
                            {
                                episode.FilePath.PathAndFilename = fileName;
                            }
                            else
                            {
                                var result =
                                    XtraMessageBox.Show(
                                        string.Format(
                                            "Do you want to replace\n{0}\nwith\n{1}", 
                                            episode.FilePath.PathAndFilename, 
                                            fileName), 
                                        "Are you sure to want to replace?", 
                                        MessageBoxButtons.YesNo, 
                                        MessageBoxIcon.Question);

                                if (result == DialogResult.Yes)
                                {
                                    episode.FilePath.PathAndFilename = fileName;
                                }
                            }

                            this.gridViewEpisodes.RefreshData();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DragOver event of the grdEpisodes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.
        /// </param>
        private void grdEpisodes_DragOver(object sender, DragEventArgs e)
        {
            object fileNameW = e.Data.GetData("FileNameW");
            if (fileNameW != null)
            {
                var fileNames = (string[])fileNameW;
                if (fileNames.Length == 1)
                {
                    string fileName = fileNames[0];

                    if (
                        Get.InOutCollection.VideoExtentions.Contains(
                            Path.GetExtension(fileName.ToLower()).Replace(".", string.Empty)))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Paint event of the grdEpisodes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.
        /// </param>
        private void grdEpisodes_Paint(object sender, PaintEventArgs e)
        {
            this.clmEpisodePath.Visible = Get.Ui.EnableTVPathColumn;
        }

        /// <summary>
        /// Handles the FocusedRowChanged event of the gridViewEpisodes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewEpisodes_FocusedRowChanged(
            object sender, FocusedRowChangedEventArgs e)
        {
            var episode = this.gridViewEpisodes.GetRow(e.FocusedRowHandle) as Episode;

            if (episode == null)
            {
                return;
            }

            TvDBFactory.SetCurrentEpisode(episode.Guid);
        }

        /// <summary>
        /// Handles the RowCellStyle event of the gridViewEpisodes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewEpisodes_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var episode = this.gridViewEpisodes.GetRow(e.RowHandle) as Episode;

            if (episode == null)
            {
                return;
            }

            if (episode.ChangedText || episode.ChangedScreenshot)
            {
                e.Appearance.Font = Get.LookAndFeel.TextChanged;
            }
            else
            {
                e.Appearance.Font = Get.LookAndFeel.TextNormal;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the gridViewEpisodes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.Data.SelectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewEpisodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var episodeList =
                this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode).ToList();

            TvDBFactory.CurrentSelectedEpisode = episodeList;
        }

        /// <summary>
        /// Handles the FocusedRowChanged event of the gridViewSeasons control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewSeasons_FocusedRowChanged(
            object sender, FocusedRowChangedEventArgs e)
        {
            var season = this.gridViewSeasons.GetRow(e.FocusedRowHandle) as Season;

            if (season == null)
            {
                return;
            }

            TvDBFactory.SetCurrentSeason(season.Guid);

            TvDBFactory.DefaultCurrentEpisode();
        }

        /// <summary>
        /// Handles the RowCellStyle event of the gridViewSeasons control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewSeasons_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var season = this.gridViewSeasons.GetRow(e.RowHandle) as Season;

            if (season == null)
            {
                return;
            }

            e.Appearance.Font = season.ContainsChangedEpisodes()
                                    ? Get.LookAndFeel.TextChanged
                                    : Get.LookAndFeel.TextNormal;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the gridViewSeasons control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.Data.SelectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewSeasons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seasonsList =
                this.gridViewSeasons.GetSelectedRows().Select(row => this.gridViewSeasons.GetRow(row) as Season).ToList();

            TvDBFactory.CurrentSelectedSeason = seasonsList;
        }

        /// <summary>
        /// Handles the FocusedRowChanged event of the gridViewTvTitleList control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewTvTitleList_FocusedRowChanged(
            object sender, FocusedRowChangedEventArgs e)
        {
            var row = this.gridViewTvTitleList.GetRow(e.FocusedRowHandle) as MasterSeriesListModel;

            if (row == null)
            {
                return;
            }

            TvDBFactory.SetCurrentSeries(row.SeriesGuid);

            TvDBFactory.DefaultCurrentSeasonAndEpisode();
        }

        /// <summary>
        /// Handles the PopupMenuShowing event of the gridViewTvTitleList control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewTvTitleList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            e.Allow = false;

            this.popupSeries.ShowPopup(this.barManager1, view.GridControl.PointToScreen(e.Point));
        }

        /// <summary>
        /// Handles the RowCellStyle event of the gridViewTvTitleList control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewTvTitleList_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var row = this.gridViewTvTitleList.GetRow(e.RowHandle) as MasterSeriesListModel;

            if (row == null)
            {
                return;
            }

            var series = TvDBFactory.GetSeriesFromGuid(row.SeriesGuid);

            if (series.ChangedText || series.ChangedPoster || series.ChangedFanart || series.ChangedBanner)
            {
                e.Appearance.Font = Get.LookAndFeel.TextChanged;
            }
            else
            {
                e.Appearance.Font = Get.LookAndFeel.TextNormal;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the gridViewTvTitleList control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.Data.SelectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void gridViewTvTitleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seriesList =
                this.gridViewTvTitleList.GetSelectedRows().Select(
                    row => this.gridViewTvTitleList.GetRow(row) as MasterSeriesListModel).Select(
                        seriesListModel => TvDBFactory.GetSeriesFromGuid(seriesListModel.SeriesGuid)).ToList();

            TvDBFactory.CurrentSelectedSeries = seriesList;
        }

        /// <summary>
        /// Handles the BeforePopup event of the popupSeries control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.
        /// </param>
        private void popupSeries_BeforePopup(object sender, CancelEventArgs e)
        {
            var rows = this.gridViewTvTitleList.GetSelectedRows();
            rows.Select(row => this.gridViewTvTitleList.GetRow(row) as MasterSeriesListModel).ToList();

            this.popupSeries.ClearLinks();

            var addBarItem = new BarSubItem(this.barManager1, "Add Series") { Glyph = Resources.add32 };
            this.popupSeries.AddItem(addBarItem);

            addBarItem.AddItem(new BarButtonItem(this.barManager1, "Add TvDB Series"));
            addBarItem.AddItem(new BarButtonItem(this.barManager1, "Add Custom Series"));

            var updateBarItem = new BarSubItem(this.barManager1, "Update Series") { Glyph = Resources.globe32 };
            this.popupSeries.AddItem(updateBarItem);

            updateBarItem.AddItem(new BarButtonItem(this.barManager1, "From TvDB"));

            var hideItem = new BarButtonItem(this.barManager1, "Hide") { Glyph = Resources.find1 };
            hideItem.ItemClick += this.hideItem_ItemClick;
            this.popupSeries.AddItem(hideItem);

            var deleteItem = new BarButtonItem(this.barManager1, "Remove") { Glyph = Resources.find1 };
            deleteItem.ItemClick += this.deleteItem_ItemClick;
            this.popupSeries.AddItem(deleteItem);
        }

        private void deleteItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewTvTitleList.GetSelectedRows().Select(row => this.gridViewTvTitleList.GetRow(row) as MasterSeriesListModel)
                .ToList().ForEach(TvDBFactory.DeleteSeries);
        }

        private void hideItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewTvTitleList.GetSelectedRows().Select(row => this.gridViewTvTitleList.GetRow(row) as MasterSeriesListModel)
                .ToList().ForEach(TvDBFactory.SetSeriesHide);
        }

        /// <summary>
        /// Handles the Tick event of the timer1 control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Factories.InOut.OutFactory.RefreshGrids)
            {
                this.gridViewTvTitleList.RefreshData();
                this.gridViewSeasons.RefreshData();
                this.gridViewEpisodes.RefreshData();
                Factories.InOut.OutFactory.RefreshGrids = false;
            }
        }

        /// <summary>
        /// Handles the GetActiveObjectInfo event of the toolTipController1 control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs"/> instance containing the event data.
        /// </param>
        private void toolTipController1_GetActiveObjectInfo(
            object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            if (e.SelectedControl == this.grdTvTitleList)
            {
                GridHitInfo hi = this.gridViewTvTitleList.CalcHitInfo(e.ControlMousePosition);
                if (hi.InRowCell)
                {
                    var series = this.gridViewTvTitleList.GetRow(hi.RowHandle) as MasterSeriesListModel;
                    e.Info = new ToolTipControlInfo(hi + " " + hi.Column.Name + " " + hi.RowHandle, string.Empty)
                        {
                           SuperTip = TvDBFactory.GetSeriesSuperTip(series.SeriesGuid) 
                        };
                }
            }
            else if (e.SelectedControl == this.grdSeasons)
            {
                GridHitInfo hi = this.gridViewSeasons.CalcHitInfo(e.ControlMousePosition);
                if (hi.InRowCell)
                {
                    var season = this.gridViewSeasons.GetRow(hi.RowHandle) as Season;
                    e.Info = new ToolTipControlInfo(hi + " " + hi.Column.Name + " " + hi.RowHandle, string.Empty)
                        {
                           SuperTip = TvDBFactory.GetSeasonSuperTip(season) 
                        };
                }
            }
            else if (e.SelectedControl == this.grdEpisodes)
            {
                GridHitInfo hi = this.gridViewEpisodes.CalcHitInfo(e.ControlMousePosition);
                if (hi.InRowCell)
                {
                    var episode = this.gridViewEpisodes.GetRow(hi.RowHandle) as Episode;
                    e.Info = new ToolTipControlInfo(hi + " " + hi.Column.Name + " " + hi.RowHandle, string.Empty)
                        {
                           SuperTip = TvDBFactory.GetEpisodeSuperTip(episode) 
                        };
                }
            }
        }

        #endregion

        private void gridViewSeasons_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            e.Allow = false;

            this.popupSeason.ShowPopup(this.barManager1, view.GridControl.PointToScreen(e.Point));
        }

        private void gridViewEpisodes_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;
            e.Allow = false;

            this.popupEpisode.ShowPopup(this.barManager1, view.GridControl.PointToScreen(e.Point));
        }

        private void popupSeasonLock_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewSeasons.GetSelectedRows().Select(row => this.gridViewSeasons.GetRow(row) as Season)
                .ToList().ForEach(TvDBFactory.LockSeason);
        }

        private void popupSeasonUnlock_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewSeasons.GetSelectedRows().Select(row => this.gridViewSeasons.GetRow(row) as Season)
                .ToList().ForEach(TvDBFactory.UnlockSeason);
        }

        private void popupSeasonHide_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewSeasons.GetSelectedRows().Select(row => this.gridViewSeasons.GetRow(row) as Season)
                .ToList().ForEach(TvDBFactory.UnlockSeason);
        }

        private void popupEpisodeOpenShow_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
                .ToList().ForEach(TvDBFactory.OpenEpisodeFile);
        }

        private void popupEpisodeOpenFolder_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
                .ToList().ForEach(TvDBFactory.OpenEpisodeFolder);
        }

        private void popupEpisodeLock_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
                .ToList().ForEach(TvDBFactory.LockEpisode);
        }

        private void popupEpisodeUnlock_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
               .ToList().ForEach(TvDBFactory.UnlockEpisode);
        }

        private void popupEpisodeHide_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
                .ToList().ForEach(TvDBFactory.HideEpisode);
        }

        private void popupEpisodeWatched_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
                .ToList().ForEach(TvDBFactory.SetEpisodeWatched);
        }

        private void popupEpisodeUnwatched_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode)
                .ToList().ForEach(TvDBFactory.SetEpisodeUnwatched);
        }

        private void popupSeason_BeforePopup(object sender, CancelEventArgs e)
        {
            var selectedSeasons = this.gridViewSeasons.GetSelectedRows().Select(row => this.gridViewSeasons.GetRow(row) as Season).ToList();

            if (selectedSeasons.Count == 1)
            {
                this.popupSeasonLock.Visibility = selectedSeasons[0].IsLocked ? BarItemVisibility.Never : BarItemVisibility.Always;
                this.popupSeasonUnlock.Visibility = selectedSeasons[0].IsLocked ? BarItemVisibility.Always : BarItemVisibility.Never;
            }
        }

        private void popupEpisode_BeforePopup(object sender, CancelEventArgs e)
        {
            var selectedEpisodes = this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode).ToList();

            if (selectedEpisodes.Count == 1)
            {
                this.popupEpisodeLock.Visibility = selectedEpisodes[0].IsLocked ? BarItemVisibility.Never : BarItemVisibility.Always;
                this.popupEpisodeUnlock.Visibility = selectedEpisodes[0].IsLocked ? BarItemVisibility.Always : BarItemVisibility.Never;

                this.popupEpisodeWatched.Visibility = selectedEpisodes[0].Watched ? BarItemVisibility.Never : BarItemVisibility.Always;
                this.popupEpisodeUnwatched.Visibility = selectedEpisodes[0].Watched ? BarItemVisibility.Always : BarItemVisibility.Never;

                if (File.Exists(selectedEpisodes[0].FilePath.PathAndFilename))
                {
                    this.popupEpisodeOpenShow.Visibility = BarItemVisibility.Always;
                    this.popupEpisodeOpenFolder.Visibility = BarItemVisibility.Always;
                }
                else
                {
                    this.popupEpisodeOpenShow.Visibility = BarItemVisibility.Never;
                    this.popupEpisodeOpenFolder.Visibility = BarItemVisibility.Never;
                    this.popupEpisodeWatched.Visibility = BarItemVisibility.Never;
                    this.popupEpisodeUnwatched.Visibility = BarItemVisibility.Never; 
                }
            }
            else
            {
                this.popupEpisodeOpenShow.Visibility = BarItemVisibility.Never;
                this.popupEpisodeOpenFolder.Visibility = BarItemVisibility.Never;
            }
        }

        private void btnUpdateEpisodes_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedEpisodes = this.gridViewEpisodes.GetSelectedRows().Select(row => this.gridViewEpisodes.GetRow(row) as Episode).ToList();

            var update = new UI.Dialogs.TV.FrmUpdateShows(selectedEpisodes, true);
            update.ShowDialog();

            this.UpdateEpisodes();
        }

        private void btnUpdateSeason_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedSeasons = this.gridViewSeasons.GetSelectedRows().Select(row => this.gridViewSeasons.GetRow(row) as Season).ToList();

            var update = new UI.Dialogs.TV.FrmUpdateShows(selectedSeasons, true);
            update.ShowDialog();

            this.UpdateSeasons();
        }
    }
}