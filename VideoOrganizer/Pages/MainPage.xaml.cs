using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoOrganizer.Model;
using VideoOrganizer.Service;

namespace VideoOrganizer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private DatabaseService dbService;
        private List<VideoModel> videos = new List<VideoModel>();
        private LogService Logger;
        private VideoModel currVideo;
        private NReco.VideoConverter.FFMpegConverter ffMpeg;

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;
            dbService = DatabaseService.Instance;
            videos = dbService.FindAllVideos();
            currVideo = null;
            if (videos != null)
            {
                lvOrganize.ItemsSource = dbService.FindAllVideos();
            }

            Logger = new LogService();
            LogTextBlock.DataContext = Logger;
            ThreadPool.SetMinThreads(100, 100);

            ffMpeg = new NReco.VideoConverter.FFMpegConverter();
        }
        
        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //gets the path of drag and drop file
                //TODO: handle directory drag and drop

                //iterate through each file in directory
                String[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                await Task<Tuple<int, int>>.Factory.StartNew(() =>
                  {
                      int validFiles = 0, invalidFiles = 0;
                      foreach (String i in files)
                      {
                          Tuple<int, int> result = ImportFiles(i);
                          validFiles += result.Item1;
                          invalidFiles += result.Item2;
                      }
                      return Tuple.Create<int, int>(validFiles, invalidFiles);
                  }).ContinueWith((result) =>
               {
                      Logger.Log(string.Format("Imported {0} files", result.Result.Item1));
                      if (result.Result.Item2 != 0)
                          Logger.Log(string.Format("Failed to import {0} files", result.Result.Item2));
                      Dispatcher.Invoke(() => lvOrganize.ItemsSource = dbService.FindAllVideos());
                  });
            }
        }

        /// <summary>
        /// Analyzes and imports files from the paths provided. 
        /// </summary>
        /// <param name="path">String array of paths</param>
        /// <returns>Tuple of valid files imported and invalid files</returns>
        private Tuple<int, int> ImportFiles(string path)
        {
            //i've been playing around with tuples in this class lol
            int validFiles = 0, invalidFiles = 0;
            var fileName = path.Substring(path.LastIndexOf('\\') + 1);
            FileAttributes attr = File.GetAttributes(@path);
            FileInfo fileInfo = new FileInfo(path);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                //handles directories and files within directory
                List<string> directories = Directory.GetDirectories(path).ToList();
                List<string> files = Directory.GetFiles(path).ToList();
                List<Task<Tuple<int,int>>> tasks = new List<Task<Tuple<int, int>>>();

                //Logger.Log(directories.Aggregate("", (acc, x) => acc += " \n" + x));

                Task<Tuple<int,int>> t1 = Task<Tuple<int, int>>.Factory.StartNew(() =>
                {
                    int TaskValidFiles=0, TaskInvalidFiles = 0;
                    files.ForEach(file =>
                    {
                        Tuple<int, int> result = ImportFiles(file);
                        TaskValidFiles += result.Item1;
                        TaskInvalidFiles += result.Item2;

                    });

                     return Tuple.Create<int, int>(TaskValidFiles,TaskInvalidFiles);
                }, TaskCreationOptions.AttachedToParent);
                tasks.Add(t1);
                
                directories.ForEach(directory =>
                {
                    Task<Tuple<int,int>> t2 = Task<Tuple<int,int>>.Factory.StartNew(() =>
                    {
                        int TaskValidFiles = 0, TaskInvalidFiles = 0;
                        Logger.Log(string.Format("Found directory {0}", directory));
                        Tuple<int, int> result = ImportFiles(directory);
                        TaskValidFiles += result.Item1;
                        TaskInvalidFiles += result.Item2;

                        return Tuple.Create<int, int>(TaskValidFiles, TaskInvalidFiles);
                    }, TaskCreationOptions.AttachedToParent);
                    tasks.Add(t2);
                });

                Task.WaitAll(tasks.ToArray());
                
                tasks.ForEach(task =>
                {
                    validFiles += task.Result.Item1;
                    invalidFiles += task.Result.Item2;
                });
                
            }
            else
            {
                //handles single file
                var inputFile = new MediaFile { Filename = @path };
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    if (inputFile.Metadata == null ||
                        inputFile.Metadata.AudioData == null || inputFile.Metadata.VideoData == null|| 
                        inputFile.Metadata.Duration.Equals(TimeSpan.Zero))
                    {
                        invalidFiles++;
                        Task.Run(() => Logger.LogAsync(string.Format("Failed to import {0}", fileName)));
                        return Tuple.Create<int, int>(validFiles, invalidFiles);
                    }
                    validFiles++;
                }
                long fileSize = new FileInfo(path).Length;
                long fileFps = Convert.ToInt64(inputFile.Metadata.VideoData.Fps);
                string fileResolution = inputFile.Metadata.VideoData.FrameSize;
                double fileDuration = inputFile.Metadata.Duration.TotalSeconds;
                DateTime dateOriginal = fileInfo.LastWriteTime;

                dbService.AddVideo(fileName, path, fileSize.ToString(), fileResolution,
                        fileFps, fileDuration.ToString(), "", dateOriginal, new DateTime());
                //lvOrganize.ItemsSource = dbService.FindAllVideos();
            }

            return Tuple.Create<int, int>(validFiles, invalidFiles);
        }

        private void SearchVideos()
        {
            string searchText = tbSearch.Text;
            lvOrganize.ItemsSource = dbService.FindVideos(searchText);
        }

        private void EditVideo()
        {
            //TODO: implement editing a video properties (category and tag)
            throw new NotImplementedException();
        }

        private void SetupEditPage()
        {
            if(currVideo == null) return;
            lbEditName.DataContext = currVideo;
            lbEditFilePath.DataContext = currVideo;
            lbEditDateAdded.DataContext = currVideo;
            lbEditDateLastWatched.DataContext = currVideo;
            lbEditDuration.DataContext = currVideo;
            lbEditFavorite.DataContext = currVideo;
            lbEditFileSize.DataContext = currVideo;
            lbEditFps.DataContext = currVideo;
            lbEditPlayCount.DataContext = currVideo;
            lbEditRating.DataContext = currVideo;
            lbEditResolution.DataContext = currVideo;
            lbEditOriginalDate.DataContext = currVideo;

            /*
            var MemStream = new MemoryStream();
            var thumbNailSource = new BitmapImage();
            ffMpeg.GetVideoThumbnail(currVideo.Path, "C:/temp/thumbnail.jpeg", 60f);
            MemStream.Position = 0;
            thumbNailSource.BeginInit();
            thumbNailSource.StreamSource = MemStream;
            thumbNailSource.EndInit();
            videoThumbnail.Source = thumbNailSource;
            */
            var thumbNailSource = new BitmapImage();
            using (var memStream = new MemoryStream())
            {
                ffMpeg.GetVideoThumbnail(currVideo.Path, memStream, 60f);
                memStream.Position = 0;
                thumbNailSource.BeginInit();
                thumbNailSource.CacheOption = BitmapCacheOption.OnLoad;
                thumbNailSource.StreamSource = memStream;
                thumbNailSource.EndInit();
                thumbNailSource.Freeze();
                videoThumbnail.Source = thumbNailSource;
            }

            ChildGrid.ColumnDefinitions[2].Width = new GridLength(this.WindowWidth / 3);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchVideos();
        }

        private void tbSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SearchVideos();
            }
        }

        private void lvOrganize_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Doubleclick functionality on list view opens edit window
            if (sender == null) return;
            ListView listView = sender as ListView;
            VideoModel selected = listView.SelectedItem as VideoModel;

            currVideo = selected;
            SetupEditPage();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Edit button click
            if (lvOrganize.SelectedItem != null)
            {
                currVideo = lvOrganize.SelectedItem as VideoModel;
                SetupEditPage();
            }

        }

        private void btnCloseEdit_Click(object sender, RoutedEventArgs e)
        {
            ChildGrid.ColumnDefinitions[2].Width = new GridLength(0);
        }

        private void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            dbService.UpdateVideo(currVideo);
        }
    }
}
