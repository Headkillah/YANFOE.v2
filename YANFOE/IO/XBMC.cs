﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="The YANFOE Project" file="XBMC.cs">
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
//   XBMC IO Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace YANFOE.IO
{
    #region Required Namespaces

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    using YANFOE.Factories.InOut.Enum;
    using YANFOE.Factories.Renamer;
    using YANFOE.Factories.Sets;
    using YANFOE.Models.MovieModels;
    using YANFOE.Models.NFOModels;
    using YANFOE.Models.SetsModels;
    using YANFOE.Models.TvModels.Show;
    using YANFOE.Settings;
    using YANFOE.Tools;
    using YANFOE.Tools.Enums;
    using YANFOE.Tools.Extentions;
    using YANFOE.Tools.Importing;
    using YANFOE.Tools.IO;
    using YANFOE.Tools.Models;
    using YANFOE.Tools.UI;
    using YANFOE.Tools.Xml;

    #endregion

    /// <summary>
    ///   XBMC IO Handler
    /// </summary>
    public class XBMC : IoBase, IOInterface
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="XBMC" /> class.
        /// </summary>
        public XBMC()
        {
            this.Type = NFOType.XBMC;
            this.ShowInSettings = true;
            this.IOHandlerName = "XBMC";
            this.IOHandlerDescription = "IO Handler for XMBC";
            this.IOHandlerUri = new Uri("http://xbmc.org/");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Generates the movie output.
        /// </summary>
        /// <param name="movieModel">
        /// The movie model. 
        /// </param>
        /// <returns>
        /// Generates a Movie NFO 
        /// </returns>
        public string GenerateMovieOutput(MovieModel movieModel)
        {
            using (var stringWriter = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, this.GetSettings()))
                {
                    this.XmlWriterStart(xmlWriter);

                    xmlWriter.WriteStartElement("movie");

                    // Title
                    XWrite.WriteEnclosedElement(xmlWriter, "title", movieModel.Title);

                    // Original title
                    XWrite.WriteEnclosedElement(xmlWriter, "originaltitle", movieModel.OriginalTitle);

                    // Sort title
                    // XWrite.WriteEnclosedElement(xmlWriter, "sorttitle", movieModel.SortTitle); // Support needed

                    // Sets
                    List<SetReturnModel> sets = MovieSetManager.GetSetReturnList(movieModel);
                    if (sets.Count > 0)
                    {
                        foreach (SetReturnModel set in sets)
                        {
                            // I'm not sure set order is supported by XBMC, however, sorttile after a set seem to. See: http://forum.xbmc.org/showthread.php?t=103441
                            XWrite.WriteEnclosedElement(xmlWriter, "set", set.SetName);
                        }
                    }

                    // Rating
                    XWrite.WriteEnclosedElement(
                        xmlWriter, "rating", this.ProcessRating(movieModel.Rating).Replace(",", "."));

                    // Year
                    XWrite.WriteEnclosedElement(xmlWriter, "year", movieModel.Year);

                    // Release Date
                    XWrite.WriteEnclosedElement(
                        xmlWriter, "releasedate", this.ProcessReleaseDate(movieModel.ReleaseDate));

                    // Top 250
                    XWrite.WriteEnclosedElement(xmlWriter, "top250", movieModel.Top250);

                    // Votes
                    XWrite.WriteEnclosedElement(xmlWriter, "votes", movieModel.Votes);

                    // Outline
                    XWrite.WriteEnclosedElement(xmlWriter, "outline", movieModel.Outline);

                    // Plot
                    XWrite.WriteEnclosedElement(xmlWriter, "plot", movieModel.Plot);

                    // Tagline
                    XWrite.WriteEnclosedElement(xmlWriter, "tagline", movieModel.Tagline);

                    // Runtime
                    XWrite.WriteEnclosedElement(
                        xmlWriter, "runtime", this.ProcessRuntime(movieModel.Runtime, movieModel.RuntimeInHourMin));

                    // Thumb
                    /*foreach (string thumb in movieModel.)
                    {
                        XWrite.WriteEnclosedElement(xmlWriter, "thumb", thumb);
                    }*/

                    // Fanart
                    // Ember Media Manager stores this as
                    /*
                     * <fanart url="http://images.themoviedb.org">
                     *  <thumb preview="http://cf1.imgobject.com/backdrops/065/4bc9af72017a3c181a000065/a-time-to-kill-original_thumb.jpg">
                     *      http://cf1.imgobject.com/backdrops/065/4bc9af72017a3c181a000065/a-time-to-kill-original.jpg
                     *  </thumb>
                     * </fanart>
                     */

                    // Mpaa
                    XWrite.WriteEnclosedElement(xmlWriter, "mpaa", movieModel.Mpaa);

                    // Certification
                    XWrite.WriteEnclosedElement(xmlWriter, "certification", movieModel.Certification);

                    // Playcount
                    // XWrite.WriteEnclosedElement(xmlWriter, "playcount", movieModel.PlayCount); // Support needed

                    // Watched
                    // http://forum.xbmc.org/showthread.php?p=747648
                    // Maybe this should be replaced by <playcount>1</playcount>
                    XWrite.WriteEnclosedElement(xmlWriter, "watched", movieModel.Watched);

                    // Imdb MovieUniqueId
                    string imdbid = movieModel.ImdbId;
                    if (!string.IsNullOrEmpty(imdbid))
                    {
                        imdbid = string.Format("tt{0}", imdbid);
                    }

                    XWrite.WriteEnclosedElement(xmlWriter, "id", imdbid);

                    // Trailer
                    try
                    {
                        if (movieModel.TrailerPathOnDisk != null && !movieModel.TrailerPathOnDisk.Equals(string.Empty))
                        {
                            XWrite.WriteEnclosedElement(xmlWriter, "trailer", movieModel.TrailerPathOnDisk);
                        }
                        else
                        {
                            if (movieModel.CurrentTrailerUrl != null
                                && !movieModel.CurrentTrailerUrl.Equals(string.Empty))
                            {
                                XWrite.WriteEnclosedElement(xmlWriter, "trailer", movieModel.CurrentTrailerUrl);
                            }
                            else
                            {
                                if (movieModel.AlternativeTrailers.Count > 0)
                                {
                                    XWrite.WriteEnclosedElement(
                                        xmlWriter, "trailer", movieModel.AlternativeTrailers[0].ToString());
                                }
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {
                    }

                    // Genre
                    foreach (string genre in movieModel.Genre)
                    {
                        XWrite.WriteEnclosedElement(xmlWriter, "genre", genre);
                    }

                    // Credits
                    if (movieModel.Writers.Count > 0)
                    {
                        xmlWriter.WriteStartElement("credits");
                        foreach (PersonModel writer in movieModel.Writers)
                        {
                            XWrite.WriteEnclosedElement(xmlWriter, "writer", writer.Name);
                        }

                        xmlWriter.WriteEndElement();
                    }

                    // Director
                    foreach (PersonModel director in movieModel.Director)
                    {
                        XWrite.WriteEnclosedElement(xmlWriter, "director", director.Name);
                    }

                    // Actor
                    int count = 1;
                    foreach (PersonModel actor in movieModel.Cast)
                    {
                        count++;

                        xmlWriter.WriteStartElement("actor");

                        string role = actor.Role;
                        if (Get.InOutCollection.CleanActorRoles)
                        {
                            role = Regex.Replace(actor.Role, @"\(as.*?\)", string.Empty).Trim();
                        }

                        XWrite.WriteEnclosedElement(xmlWriter, "name", actor.Name);
                        XWrite.WriteEnclosedElement(xmlWriter, "role", role);
                        XWrite.WriteEnclosedElement(xmlWriter, "thumb", actor.ImageUrl);

                        xmlWriter.WriteEndElement();

                        if (count == 10)
                        {
                            break;
                        }
                    }

                    // Unused in XBMC?
                    // Tmdb MovieUniqueId
                    // XWrite.WriteEnclosedElement(xmlWriter, "id", movieModel.TmdbId, "moviedb", "tmdb");

                    // Company
                    XWrite.WriteEnclosedElement(xmlWriter, "studio", movieModel.SetStudio);

                    // Country
                    foreach (var country in movieModel.Country)
                    {
                        XWrite.WriteEnclosedElement(xmlWriter, "country", country);
                    }

                    // Unused in XBMC?
                    // XWrite.WriteEnclosedElement(xmlWriter, "videosource", movieModel.VideoSource);

                    // FileInfo
                    XWrite.WriteEnclosedElement(xmlWriter, "fileinfo", "template");

                    xmlWriter.WriteEndElement();
                }

                return stringWriter.ToString().Replace(
                    "<fileinfo>template</fileinfo>", this.GetFileInfo(movieModel));
            }
        }

        /// <summary>
        /// Generates the series output.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Generates a XML output 
        /// </returns>
        public string GenerateSeriesOutput(Series series)
        {
            // http://wiki.xbmc.org/index.php?title=Import_-_Export_Library#TV_Shows
            using (var stringWriter = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, this.GetSettings()))
                {
                    this.XmlWriterStart(xmlWriter);

                    xmlWriter.WriteStartElement("tvshow");

                    // Id
                    XWrite.WriteEnclosedElement(xmlWriter, "id", series.SeriesID);

                    // Title
                    XWrite.WriteEnclosedElement(xmlWriter, "title", series.SeriesName);

                    // Rating
                    XWrite.WriteEnclosedElement(xmlWriter, "rating", series.Rating);

                    // Certification
                    XWrite.WriteEnclosedElement(xmlWriter, "mpaa", series.ContentRating);

                    // Votes
                    // XWrite.WriteEnclosedElement(xmlWriter, "votes", series.Votes);

                    // Plot
                    XWrite.WriteEnclosedElement(xmlWriter, "plot", series.Overview);

                    // Runtime
                    XWrite.WriteEnclosedElement(xmlWriter, "runtime", series.Runtime);

                    // Tagline
                    // XWrite.WriteEnclosedElement(xmlWriter, "tagline", series.Tagline);

                    // Thumb

                    // Fanart

                    // Episodeguide
                    xmlWriter.WriteStartElement("episodeguide");

                    // XWrite.WriteEnclosedElement(xmlWriter, "url", series.EpisodeGuideUrl); // Cache attribute supported: <url cache="73388.xml">http://www.thetvdb.com/api/1D62F2F90030C444/series/73388/all/en.zip</url>
                    xmlWriter.WriteEndElement();

                    // Genre
                    foreach (string genre in series.Genre)
                    {
                        XWrite.WriteEnclosedElement(xmlWriter, "genre", genre);
                    }

                    // Director
                    // XWrite.WriteEnclosedElement(xmlWriter, "director", series.Director);

                    // Premiered
                    if (series.FirstAired != null)
                    {
                        XWrite.WriteEnclosedElement(
                            xmlWriter, "premiered", series.FirstAired.Value.ToString("yyyy-MM-dd"));
                    }

                    // Status
                    XWrite.WriteEnclosedElement(xmlWriter, "aired", series.Status);

                    // Aired
                    XWrite.WriteEnclosedElement(xmlWriter, "aired", series.FirstAired);

                    // Studio
                    XWrite.WriteEnclosedElement(xmlWriter, "studio", series.Network);

                    // Trailer
                    // XWrite.WriteEnclosedElement(xmlWriter, "trailer", series.Trailer);

                    // Actor
                    foreach (PersonModel actor in series.Actors)
                    {
                        string role = actor.Role;
                        if (Get.InOutCollection.CleanActorRoles)
                        {
                            role = Regex.Replace(actor.Role, @"\(as.*?\)", string.Empty).Trim();
                        }

                        xmlWriter.WriteStartElement("actor");
                        XWrite.WriteEnclosedElement(xmlWriter, "name", actor.Name);
                        XWrite.WriteEnclosedElement(xmlWriter, "role", role);
                        XWrite.WriteEnclosedElement(xmlWriter, "thumb", actor.ImageUrl);
                        xmlWriter.WriteEndElement();
                    }

                    // Unused in XBMC?
                    // Country
                    // XWrite.WriteEnclosedElement(xmlWriter, "country", series.Country);
                    xmlWriter.WriteEndElement();
                }

                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Generates the single episode output.
        /// </summary>
        /// <param name="episode">
        /// The episode. 
        /// </param>
        /// <param name="writeDocumentTags">
        /// if set to <c>true</c> [write document tags]. 
        /// </param>
        /// <returns>
        /// Episode Output 
        /// </returns>
        public string GenerateSingleEpisodeOutput(Episode episode, bool writeDocumentTags)
        {
            using (var stringWriter = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, this.GetSettings()))
                {
                    if (writeDocumentTags)
                    {
                        this.XmlWriterStart(xmlWriter);
                    }

                    xmlWriter.WriteStartElement("episodedetails");

                    // Season
                    int? sn = episode.SeasonNumber;
                    if (sn == null || sn < 0)
                    {
                        sn = 0;
                    }

                    XWrite.WriteEnclosedElement(xmlWriter, "season", sn);

                    // Episode
                    XWrite.WriteEnclosedElement(xmlWriter, "episode", episode.EpisodeNumber);

                    // Title
                    XWrite.WriteEnclosedElement(xmlWriter, "title", episode.EpisodeName);

                    // Rating
                    XWrite.WriteEnclosedElement(xmlWriter, "rating", episode.Rating);

                    // Plot
                    XWrite.WriteEnclosedElement(xmlWriter, "plot", episode.Overview);

                    // Thumb

                    // Playcount
                    // XWrite.WriteEnclosedElement(xmlWriter, "playcount", episode.PlayCount);

                    // Lastplayed
                    // XWrite.WriteEnclosedElement(xmlWriter, "lastplayed", episode.LastPlayed);

                    // Credits
                    // XWrite.WriteEnclosedElement(xmlWriter, "credits", episode.Credits);

                    // Director
                    foreach (PersonModel director in episode.Director)
                    {
                        XWrite.WriteEnclosedElement(xmlWriter, "director", director.Name);
                    }

                    // Aired
                    XWrite.WriteEnclosedElement(xmlWriter, "aired", episode.FirstAired);

                    // Premiered
                    // XWrite.WriteEnclosedElement(xmlWriter, "premiered", episode.Premiered);

                    // Studio
                    // XWrite.WriteEnclosedElement(xmlWriter, "studio", episode.Studio);

                    // Mpaa
                    // XWrite.WriteEnclosedElement(xmlWriter, "mpaa", episode.Mpaa);

                    // Displayepisode: For TV show specials, determines how the episode is sorted in the series
                    // XWrite.WriteEnclosedElement(xmlWriter, "displayepisode", episode.DisplayEpisode);

                    // Actor
                    int count = 1;
                    foreach (PersonModel actor in episode.GuestStars)
                    {
                        count++;

                        xmlWriter.WriteStartElement("actor");

                        string role = actor.Role;
                        if (Get.InOutCollection.CleanActorRoles)
                        {
                            role = Regex.Replace(actor.Role, @"\(as.*?\)", string.Empty).Trim();
                        }

                        XWrite.WriteEnclosedElement(xmlWriter, "name", actor.Name);
                        XWrite.WriteEnclosedElement(xmlWriter, "role", role);
                        XWrite.WriteEnclosedElement(xmlWriter, "thumb", actor.ImageUrl);

                        xmlWriter.WriteEndElement();

                        if (count == 10)
                        {
                            break;
                        }
                    }

                    // Fileinfo
                    XWrite.WriteEnclosedElement(xmlWriter, "fileinfo", "template");

                    xmlWriter.WriteEndElement();

                    if (writeDocumentTags)
                    {
                        xmlWriter.WriteEndDocument();
                    }
                }

                return stringWriter.ToString().Replace(
                    "<fileinfo>template</fileinfo>", this.GetFileInfo(episode: episode));
            }
        }

        /// <summary>
        /// Gets the episode NFO.
        /// </summary>
        /// <param name="episode">
        /// The episode. 
        /// </param>
        /// <returns>
        /// Episode NFO path 
        /// </returns>
        public string GetEpisodeNFO(Episode episode)
        {
            string fullPath = episode.FilePath.PathAndFilename;

            string path = Path.GetDirectoryName(fullPath);
            string fileName = Path.GetFileNameWithoutExtension(fullPath);

            string checkPath = path + Path.DirectorySeparatorChar + fileName + ".nfo";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the episode screenshot.
        /// </summary>
        /// <param name="episode">
        /// The episode. 
        /// </param>
        /// <returns>
        /// Episode Screenshot path 
        /// </returns>
        public string GetEpisodeScreenshot(Episode episode)
        {
            string fullPath = episode.FilePath.PathAndFilename;

            string path = Path.GetDirectoryName(fullPath);
            string fileName = Path.GetFileNameWithoutExtension(fullPath);

            string checkPath = path + Path.DirectorySeparatorChar + fileName + ".tbn";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// The get file info.
        /// </summary>
        /// <param name="movie">
        /// The movie.
        /// </param>
        /// <param name="episode">
        /// The episode.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFileInfo(MovieModel movie = null, Episode episode = null)
        {
            FileInfoModel fileInfoModel;

            if (movie != null)
            {
                fileInfoModel = movie.FileInfo;
            }
            else if (episode != null)
            {
                fileInfoModel = episode.FileInfo;
            }
            else
            {
                return string.Empty;
            }

            string output;

            using (var stringWriter = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, this.GetSettings()))
                {
                    XWrite.WriteEnclosedElement(xmlWriter, "videooutput", Get.MediaInfo.DoReplace(fileInfoModel));
                }

                output = stringWriter + Environment.NewLine;
            }

            using (var stringWriter = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, this.GetSettings()))
                {
                    xmlWriter.WriteStartElement("fileinfo");
                    xmlWriter.WriteStartElement("streamdetails");
                    xmlWriter.WriteStartElement("video");

                    // Codec
                    XWrite.WriteEnclosedElement(xmlWriter, "codec", fileInfoModel.Codec);

                    // Aspect
                    XWrite.WriteEnclosedElement(xmlWriter, "aspect", fileInfoModel.AspectRatio);

                    // Width
                    XWrite.WriteEnclosedElement(xmlWriter, "width", fileInfoModel.Width);

                    // Height
                    XWrite.WriteEnclosedElement(xmlWriter, "height", fileInfoModel.Height);

                    // Scantype
                    string scanType = fileInfoModel.ProgressiveScan ? "Progressive" : "Interlaced";

                    XWrite.WriteEnclosedElement(xmlWriter, "scantype", scanType);

                    xmlWriter.WriteEndElement();

                    foreach (var audioStream in fileInfoModel.AudioStreams)
                    {
                        xmlWriter.WriteStartElement("audio");

                        // Codec
                        XWrite.WriteEnclosedElement(xmlWriter, "codec", audioStream.CodecId);

                        // Language
                        XWrite.WriteEnclosedElement(xmlWriter, "codec", audioStream.Language);

                        // Channels
                        XWrite.WriteEnclosedElement(
                            xmlWriter, "channels", audioStream.Channels.Replace(" channels", string.Empty));

                        xmlWriter.WriteEndElement();
                    }

                    foreach (var subtitleStream in fileInfoModel.SubtitleStreams)
                    {
                        xmlWriter.WriteStartElement("subtitle");

                        // Codec
                        XWrite.WriteEnclosedElement(xmlWriter, "language", subtitleStream.Language);

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }

                return
                    Regex.Replace(output + stringWriter, @"\<\?xml.*?\>", string.Empty).Replace(
                        Environment.NewLine + Environment.NewLine, Environment.NewLine).Trim();
            }
        }

        /// <summary>
        /// Gets the season banner.
        /// </summary>
        /// <param name="season">
        /// The season. 
        /// </param>
        /// <returns>
        /// Season banner path 
        /// </returns>
        public string GetSeasonBanner(Season season)
        {
            // Having folder.jpg inside the season folder, might overwrite the season poster named seasonxx.tbn in season root
            if (string.IsNullOrEmpty(season.GetSeasonPath()))
            {
                return string.Empty;
            }

            string path = season.GetSeasonPath();

            string checkPath = path + Path.DirectorySeparatorChar + "folder.jpg";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the season fanart.
        /// </summary>
        /// <param name="season">
        /// The season. 
        /// </param>
        /// <returns>
        /// Season fanart path 
        /// </returns>
        public string GetSeasonFanart(Season season)
        {
            // Not sure this is supported by XBMC
            string firstEpisode = season.GetFirstEpisode();

            if (string.IsNullOrEmpty(firstEpisode))
            {
                return string.Empty;
            }

            string path = Path.GetDirectoryName(firstEpisode);

            string checkPath = path + Path.DirectorySeparatorChar + "fanart.jpg";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the season poster.
        /// </summary>
        /// <param name="season">
        /// The season. 
        /// </param>
        /// <returns>
        /// Season Poster path 
        /// </returns>
        public string GetSeasonPoster(Season season)
        {
            string seasonPath = season.GetSeasonPath();

            if (string.IsNullOrEmpty(seasonPath))
            {
                return string.Empty;
            }

            string path = Path.GetDirectoryName(seasonPath);

            // <root>/<tv series>/season<00>.tbn
            // <root>/<tv series>/season <00>/<episodes>
            string checkPath = path + "season" + string.Format("{0:d2}", season.SeasonNumber) + ".tbn";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the series banner.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Series Banner path 
        /// </returns>
        public string GetSeriesBanner(Series series)
        {
            string seriesPath = series.GetSeriesPath();

            if (string.IsNullOrEmpty(seriesPath))
            {
                return string.Empty;
            }

            string checkPath = seriesPath + Path.DirectorySeparatorChar + "folder.jpg";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the series fanart.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Series Fanart path 
        /// </returns>
        public string GetSeriesFanart(Series series)
        {
            string seriesPath = series.GetSeriesPath();

            if (string.IsNullOrEmpty(seriesPath))
            {
                return string.Empty;
            }

            string checkPath = seriesPath + Path.DirectorySeparatorChar + "fanart.jpg";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the series NFO.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Series NFO path 
        /// </returns>
        public string GetSeriesNFO(Series series)
        {
            string seriesPath = series.GetSeriesPath();

            if (string.IsNullOrEmpty(seriesPath))
            {
                return string.Empty;
            }

            string checkPath = seriesPath + Path.DirectorySeparatorChar + "tvshow.nfo";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the series poster.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Series poster path 
        /// </returns>
        public string GetSeriesPoster(Series series)
        {
            string seriesPath = series.GetSeriesPath();

            if (string.IsNullOrEmpty(seriesPath))
            {
                return string.Empty;
            }

            string checkPath = seriesPath + Path.DirectorySeparatorChar + "season-all.tbn";

            if (File.Exists(checkPath))
            {
                return checkPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Loads the episode.
        /// </summary>
        /// <param name="episode">
        /// The episode. 
        /// </param>
        /// <returns>
        /// Episode Object 
        /// </returns>
        public bool LoadEpisode(Episode episode)
        {
            string episodeName = episode.EpisodeName;
            string episodePath = episode.FilePath.FolderPath;

            string nfo = Find.FindNFO(episodeName, episodePath);

            if (string.IsNullOrEmpty(nfo))
            {
                return false;
            }

            XmlDocument doc = XRead.OpenPath(nfo);

            episode.SeasonNumber = XRead.GetInt(doc, "season");
            episode.EpisodeNumber = XRead.GetInt(doc, "episode");
            episode.EpisodeName = XRead.GetString(doc, "title");
            episode.Rating = XRead.GetDouble(doc, "rating");
            episode.Overview = XRead.GetString(doc, "plot");

            // episode.PlayCount = XRead.GetInt(doc, "playcount");
            // episode.LastPlayed = XRead.GetString(doc, "lastplayed");
            // episode.Credits = XRead.GetString(doc, "credits");
            List<string> directorList = XRead.GetStrings(doc, "director");
            foreach (string director in directorList)
            {
                episode.Director.Add(new PersonModel(director));
            }

            episode.FirstAired = XRead.GetDateTime(doc, "aired");

            // episode.Premiered = XRead.GetString(doc, "premiered");
            // episode.Studio = XRead.GetString(doc, "studio");
            // episode.Mpaa = XRead.GetString(doc, "mpaa");
            // episode.DisplayEpisode = XRead.GetInt(doc, "displayepisode");

            // Actor
            if (doc.GetElementsByTagName("actor").Count > 0)
            {
                episode.GuestStars = new ThreadedBindingList<PersonModel>();

                foreach (XmlNode actor in doc.GetElementsByTagName("actor"))
                {
                    string xmlActor = actor.InnerXml;

                    XmlDocument docActor = XRead.OpenXml("<x>" + xmlActor + "</x>");

                    string name = XRead.GetString(docActor, "name");
                    string role = XRead.GetString(docActor, "role");
                    string imageurl = XRead.GetString(docActor, "thumb");

                    var personModel = new PersonModel(name, role, imageurl);

                    episode.GuestStars.Add(personModel);
                }
            }

            // Load fileinfo
            return true;
        }

        /// <summary>
        /// Loads the movie.
        /// </summary>
        /// <param name="movieModel">
        /// The movie model. 
        /// </param>
        public void LoadMovie(MovieModel movieModel)
        {
            XmlDocument xmlReader = XRead.OpenPath(movieModel.NfoPathOnDisk);

            if (xmlReader == null)
            {
                return;
            }

            // Ids
            XmlNodeList ids = xmlReader.GetElementsByTagName("id");
            foreach (XmlElement id in ids)
            {
                if (id.Attributes["moviedb"] == null)
                {
                    movieModel.ImdbId = id.InnerXml.Replace("tt", string.Empty);
                }
                else
                {
                    switch (id.Attributes["moviedb"].Value)
                    {
                        case "tmdb":
                            movieModel.TmdbId = id.InnerXml;
                            break;
                    }
                }
            }

            // Title
            movieModel.Title = XRead.GetString(xmlReader, "title");

            // Original title
            movieModel.OriginalTitle = XRead.GetString(xmlReader, "originaltitle");

            // Sort title
            // movieModel.SortTitle = XRead.GetString(xmlReader, "sorttitle");

            // Sets
            XmlNodeList sets = xmlReader.GetElementsByTagName("set");
            foreach (XmlElement set in sets)
            {
                MovieSetManager.AddMovieToSet(movieModel, set.InnerXml);
            }

            // Rating
            movieModel.Rating = XRead.GetDouble(xmlReader, "rating");

            // Year
            movieModel.Year = XRead.GetInt(xmlReader, "year");

            // Release Date
            movieModel.ReleaseDate = XRead.GetDateTime(xmlReader, "releasedate");

            // Top 250
            movieModel.Top250 = XRead.GetInt(xmlReader, "top250");

            // Votes
            movieModel.Votes = XRead.GetInt(xmlReader, "votes");

            // Outline
            movieModel.Outline = XRead.GetString(xmlReader, "outline");

            // Plot
            movieModel.Plot = XRead.GetString(xmlReader, "plot");

            // Tagline
            movieModel.Tagline = XRead.GetString(xmlReader, "tagline");

            // Runtime
            string check = XRead.GetString(xmlReader, "runtime");
            if (check.Contains("m"))
            {
                movieModel.RuntimeInHourMin = check;
            }
            else
            {
                movieModel.Runtime = XRead.GetInt(xmlReader, "runtime");
            }

            // Thumb
            /*XmlNodeList thumbs = xmlReader.GetElementsByTagName("set");
            foreach (XmlElement thumb in thumbs)
            {
                // Add thumb
            }*/

            // Fanart too

            // Mpaa 
            movieModel.Mpaa = XRead.GetString(xmlReader, "mpaa");

            // Certification
            movieModel.Certification = XRead.GetString(xmlReader, "certification");

            // Playcount
            // XWrite.WriteEnclosedElement(xmlWriter, "playcount", movieModel.PlayCount); // Support needed

            // Studio
            movieModel.SetStudio = XRead.GetString(xmlReader, "studio");

            // Watched
            movieModel.Watched = XRead.GetBool(xmlReader, "watched");

            // Trailer
            movieModel.CurrentTrailerUrl = XRead.GetString(xmlReader, "trailer");
                
                // Trailer element should go to the field that it was created from

            // Genre
            XmlNodeList genres = xmlReader.GetElementsByTagName("genre");
            if (genres.Count > 1)
            {
                foreach (XmlElement genre in genres)
                {
                    movieModel.Genre.Add(genre.InnerXml);
                }
            }
            else
            {
                string genreList = XRead.GetString(xmlReader, "genre");
                movieModel.GenreAsString = genreList.Replace(" / ", ",");
            }

            // Credits
            movieModel.Writers.Clear();
            XmlNodeList writers = xmlReader.GetElementsByTagName("writer");
            foreach (XmlElement writer in writers)
            {
                movieModel.Writers.Add(new PersonModel(writer.InnerXml));
            }

            // Director
            List<string> directorList = XRead.GetStrings(xmlReader, "director");
            foreach (string director in directorList)
            {
                movieModel.Director.Add(new PersonModel(director));
            }

            // Country
            XmlNodeList countries = xmlReader.GetElementsByTagName("country");
            foreach (XmlElement country in countries)
            {
                movieModel.Country.Add(country.InnerXml);
            }

            // Unused in XBMC?
            // Language
            // movieModel.LanguageAsString = XRead.GetString(xmlReader, "language");

            // Actors
            XmlNodeList actors = xmlReader.GetElementsByTagName("actor");
            movieModel.Cast.Clear();

            foreach (XmlElement actor in actors)
            {
                XmlDocument document = XRead.OpenXml("<x>" + actor.InnerXml + "</x>");

                string name = XRead.GetString(document, "name");
                string role = XRead.GetString(document, "role");
                string thumb = XRead.GetString(document, "thumb");

                movieModel.Cast.Add(new PersonModel(name, thumb, role));
            }

            /*var source = XRead.GetString(xmlReader, "videosource");

            if (!string.IsNullOrEmpty(source))
            {
                movieModel.VideoSource = source;
            }
            */
        }

        /// <summary>
        /// Loads the season.
        /// </summary>
        /// <param name="season">
        /// The season. 
        /// </param>
        /// <returns>
        /// Season object 
        /// </returns>
        public bool LoadSeason(Season season)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the series.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <returns>
        /// Loaded succeeded 
        /// </returns>
        public bool LoadSeries(Series series)
        {
            string seriesName = series.GetSeriesNameOnDisk();
            string seriesPath = series.GetSeriesPath();

            if (string.IsNullOrEmpty(seriesName) || string.IsNullOrEmpty(seriesPath))
            {
                return false;
            }

            string nfo = Find.FindNFO(seriesName, seriesPath);

            if (string.IsNullOrEmpty(nfo))
            {
                return false;
            }

            XmlDocument doc = XRead.OpenPath(nfo);

            series.SeriesID = XRead.GetUInt(doc, "id");
            series.SeriesName = XRead.GetString(doc, "title");
            series.Rating = XRead.GetDouble(doc, "rating");
            series.ContentRating = XRead.GetString(doc, "mpaa");

            // series.Votes = XRead.GetInt(doc, "votes");
            series.Overview = XRead.GetString(doc, "plot");
            series.Runtime = XRead.GetInt(doc, "runtime");

            // series.Tagline = XRead.GetString(doc, "tagline");
            // Thumb
            // Fanart
            // series.EpisodeGuide = XRead.GetString(doc, "url"); // url is located in episodeguide tags
            series.Genre = XRead.GetStrings(doc, "genre").ToThreadedBindingList();

            // series.Director = XRead.GetString(doc, "director");
            series.FirstAired = XRead.GetDateTime(doc, "premiered", "yyyy-MM-dd");
            series.Status = XRead.GetString(doc, "status");

            // series.Aired = XRead.GetString(doc, "aired");
            series.Network = XRead.GetString(doc, "studio");

            // series.Trailer = XRead.GetString(doc, "trailer");
            if (doc.GetElementsByTagName("actor").Count > 0)
            {
                series.Actors = new ThreadedBindingList<PersonModel>();

                foreach (XmlNode actor in doc.GetElementsByTagName("actor"))
                {
                    string xmlActor = actor.InnerXml;

                    XmlDocument docActor = XRead.OpenXml("<x>" + xmlActor + "</x>");

                    string name = XRead.GetString(docActor, "name");
                    string role = XRead.GetString(docActor, "role");
                    string imageurl = XRead.GetString(docActor, "thumb");

                    var personModel = new PersonModel(name, role, imageurl);

                    series.Actors.Add(personModel);
                }
            }

            return true;
        }

        /// <summary>
        /// Saves the episode.
        /// </summary>
        /// <param name="episode">
        /// The episode. 
        /// </param>
        /// <param name="type">
        /// The EpisodeIOType type. 
        /// </param>
        public void SaveEpisode(Episode episode, EpisodeIOType type)
        {
            if (episode.Secondary)
            {
                return;
            }

            if (string.IsNullOrEmpty(episode.FilePath.PathAndFilename))
            {
                return;
            }

            string nfoTemplate;
            string screenshotTemplate;

            if (MovieNaming.IsDVD(episode.FilePath.PathAndFilename))
            {
                nfoTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDEpisodeNFOTemplate;
                screenshotTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDEpisodeScreenshotTemplate;
            }
            else if (MovieNaming.IsBluRay(episode.FilePath.PathAndFilename))
            {
                nfoTemplate = Get.InOutCollection.CurrentTvSaveSettings.BlurayEpisodeNFOTemplate;
                screenshotTemplate = Get.InOutCollection.CurrentTvSaveSettings.BlurayEpisodeScreenshotTemplate;
            }
            else
            {
                nfoTemplate = Get.InOutCollection.CurrentTvSaveSettings.EpisodeNFOTemplate;
                screenshotTemplate = Get.InOutCollection.CurrentTvSaveSettings.EpisodeScreenshotTemplate;
            }

            // Nfo
            if (type == EpisodeIOType.All || type == EpisodeIOType.Nfo)
            {
                string nfoPathTo = GeneratePath.TvEpisode(episode, nfoTemplate, string.Empty);

                this.WriteNFO(this.GenerateSingleEpisodeOutput(episode, true), nfoPathTo);
                episode.ChangedText = false;
            }

            // Screenshot
            if (type == EpisodeIOType.Screenshot || type == EpisodeIOType.All)
            {
                if (!string.IsNullOrEmpty(episode.FilePath.PathAndFilename))
                {
                    string screenshotPathFrom;

                    if (!string.IsNullOrEmpty(episode.EpisodeScreenshotPath)
                        && File.Exists(episode.EpisodeScreenshotPath))
                    {
                        screenshotPathFrom = episode.EpisodeScreenshotPath;
                    }
                    else
                    {
                        screenshotPathFrom = this.TVPathImageGet(episode.EpisodeScreenshotUrl);
                    }

                    string screenshotPathTo = GeneratePath.TvEpisode(episode, screenshotTemplate, screenshotPathFrom);

                    this.CopyFile(screenshotPathFrom, screenshotPathTo);
                    episode.ChangedScreenshot = false;
                }
            }
        }

        /// <summary>
        /// Saves the season.
        /// </summary>
        /// <param name="season">
        /// The season. 
        /// </param>
        /// <param name="type">
        /// The SeasonIOType type. 
        /// </param>
        public void SaveSeason(Season season, SeasonIOType type)
        {
            if (season.HasEpisodeWithPath())
            {
                string posterTemplate;
                string fanartTemplate;
                string bannerTemplate;

                string firstEpisodePath = season.GetFirstEpisode();

                if (MovieNaming.IsBluRay(firstEpisodePath))
                {
                    posterTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeasonPosterTemplate;
                    fanartTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeasonFanartTemplate;
                    bannerTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeasonBannerTemplate;
                }
                else if (MovieNaming.IsDVD(firstEpisodePath))
                {
                    posterTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeasonPosterTemplate;
                    fanartTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeasonFanartTemplate;
                    bannerTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeasonBannerTemplate;
                }
                else
                {
                    posterTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeasonPosterTemplate;
                    fanartTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeasonFanartTemplate;
                    bannerTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeasonBannerTemplate;
                }

                // Poster
                if (type == SeasonIOType.All || type == SeasonIOType.Poster)
                {
                    if (!string.IsNullOrEmpty(season.PosterUrl) || string.IsNullOrEmpty(season.PosterPath))
                    {
                        string posterPathFrom;

                        if (!string.IsNullOrEmpty(season.PosterPath) && File.Exists(season.PosterPath))
                        {
                            posterPathFrom = season.PosterPath;
                        }
                        else
                        {
                            posterPathFrom = this.TVPathImageGet(season.PosterUrl);
                        }

                        string posterPathTo = GeneratePath.TvSeason(season, posterTemplate, posterPathFrom);

                        this.CopyFile(posterPathFrom, posterPathTo);
                        season.ChangedPoster = false;
                    }
                }

                // Fanart
                if (type == SeasonIOType.All || type == SeasonIOType.Fanart)
                {
                    if (!string.IsNullOrEmpty(season.FanartUrl) || !string.IsNullOrEmpty(season.FanartPath))
                    {
                        string fanartPathFrom;

                        if (!string.IsNullOrEmpty(season.FanartPath) && File.Exists(season.FanartPath))
                        {
                            fanartPathFrom = season.FanartPath;
                        }
                        else
                        {
                            fanartPathFrom = this.TVPathImageGet(season.FanartUrl);
                        }

                        string fanartPathTo = GeneratePath.TvSeason(season, fanartTemplate, fanartPathFrom);

                        this.CopyFile(fanartPathFrom, fanartPathTo);
                        season.ChangedFanart = false;
                    }
                }

                // Banner
                if (type == SeasonIOType.All || type == SeasonIOType.Banner)
                {
                    if (!string.IsNullOrEmpty(season.BannerUrl) || !string.IsNullOrEmpty(season.BannerPath))
                    {
                        string bannerPathFrom;

                        if (!string.IsNullOrEmpty(season.BannerPath) && File.Exists(season.BannerPath))
                        {
                            bannerPathFrom = season.BannerPath;
                        }
                        else
                        {
                            bannerPathFrom = this.TVPathImageGet(season.BannerUrl);
                        }

                        string bannerPathTo = GeneratePath.TvSeason(season, bannerTemplate, bannerPathFrom);

                        this.CopyFile(bannerPathFrom, bannerPathTo);
                        season.ChangedBanner = false;
                    }
                }
            }
        }

        /// <summary>
        /// Saves the series.
        /// </summary>
        /// <param name="series">
        /// The series. 
        /// </param>
        /// <param name="type">
        /// The SeriesIOType type. 
        /// </param>
        public void SaveSeries(Series series, SeriesIOType type)
        {
            string path = series.GetSeriesPath();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string nfoTemplate;
            string posterTemplate;
            string fanartTemplate;
            string bannerTemplate;

            string firstEpisodePath = series.GetFirstEpisode();

            if (Get.InOutCollection.RenameTV)
            {
                TVRenameFactory.RenameSeries(series);
            }

            if (MovieNaming.IsBluRay(firstEpisodePath))
            {
                nfoTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeriesNfoTemplate;
                posterTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeriesPosterTemplate;
                fanartTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeriesFanartTemplate;
                bannerTemplate = Get.InOutCollection.CurrentTvSaveSettings.BluraySeriesBannerTemplate;
            }
            else if (MovieNaming.IsDVD(firstEpisodePath))
            {
                nfoTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeriesNfoTemplate;
                posterTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeriesPosterTemplate;
                fanartTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeriesFanartTemplate;
                bannerTemplate = Get.InOutCollection.CurrentTvSaveSettings.DVDSeriesBannerTemplate;
            }
            else
            {
                nfoTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeriesNfoTemplate;
                posterTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeriesPosterTemplate;
                fanartTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeriesFanartTemplate;
                bannerTemplate = Get.InOutCollection.CurrentTvSaveSettings.SeriesBannerTemplate;
            }

            if (type == SeriesIOType.All || type == SeriesIOType.Nfo)
            {
                // Nfo
                string nfoPathTo = GeneratePath.TvSeries(series, nfoTemplate, string.Empty);

                this.WriteNFO(this.GenerateSeriesOutput(series), nfoPathTo);
                series.ChangedText = false;
            }

            // Poster
            if (type == SeriesIOType.All || type == SeriesIOType.Images || type == SeriesIOType.Poster)
            {
                if (!string.IsNullOrEmpty(series.PosterUrl) || !string.IsNullOrEmpty(series.PosterPath))
                {
                    string posterPathFrom;

                    if (!string.IsNullOrEmpty(series.PosterPath) && File.Exists(series.PosterPath))
                    {
                        posterPathFrom = series.PosterPath;
                    }
                    else
                    {
                        posterPathFrom = this.TVPathImageGet(series.PosterUrl);
                    }

                    string posterPathTo = GeneratePath.TvSeries(series, posterTemplate, posterPathFrom);

                    this.CopyFile(posterPathFrom, posterPathTo);
                    series.ChangedPoster = false;
                }
            }

            // Fanart
            if (type == SeriesIOType.All || type == SeriesIOType.Images || type == SeriesIOType.Fanart)
            {
                if (!string.IsNullOrEmpty(series.FanartUrl) || !string.IsNullOrEmpty(series.FanartPath))
                {
                    string fanartPathFrom;

                    if (!string.IsNullOrEmpty(series.FanartPath) && File.Exists(series.FanartPath))
                    {
                        fanartPathFrom = series.FanartPath;
                    }
                    else
                    {
                        fanartPathFrom = this.TVPathImageGet(series.FanartUrl);
                    }

                    string fanartPathTo = GeneratePath.TvSeries(series, fanartTemplate, fanartPathFrom);

                    this.CopyFile(fanartPathFrom, fanartPathTo);
                    series.ChangedFanart = false;
                }
            }

            // Banner
            if (type == SeriesIOType.All || type == SeriesIOType.Images || type == SeriesIOType.Banner)
            {
                if (!string.IsNullOrEmpty(series.SeriesBannerUrl) || !string.IsNullOrEmpty(series.SeriesBannerPath))
                {
                    string bannerPathFrom;

                    if (!string.IsNullOrEmpty(series.SeriesBannerPath) && File.Exists(series.SeriesBannerPath))
                    {
                        bannerPathFrom = series.SeriesBannerPath;
                    }
                    else
                    {
                        bannerPathFrom = this.TVPathImageGet(series.SeriesBannerUrl);
                    }

                    string bannerPathTo = GeneratePath.TvSeries(series, bannerTemplate, bannerPathFrom);

                    this.CopyFile(bannerPathFrom, bannerPathTo);
                    series.ChangedBanner = false;
                }
            }
        }

        #endregion
    }
}