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

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;
            dbService = DatabaseService.Instance;
            videos = dbService.FindAllVideos();
            if (videos != null)
            {
                lvOrganize.ItemsSource = dbService.FindAllVideos();
            }

            Logger = new LogService();
            LogTextBlock.DataContext = Logger;
            ThreadPool.SetMinThreads(100, 100);

            //videos.Add(new VideoModel() { Name = "Complete this WPF tutorial", Path = "asdg", IsFavorite=true, FileSize="500", PlayCount=1, Rating=3, Resolution="1920x1080", Fps=60, Seconds=3600, DateAdded= new TimeSpan() });
            //videos.Add(new VideoModel() { Name = "Learn C#", Path = "asdg", IsFavorite = true, FileSize = "600", PlayCount = 2, Rating = 5, Resolution = "1024x768", Fps = 30, Seconds = 3300, DateAdded = new TimeSpan() });
            //videos.Add(new VideoModel() { Name = "Wash the car", Path = "asdijuh", IsFavorite = true, FileSize = "800", PlayCount = 2, Rating = 4, Resolution = "1280x1024", Fps = 120, Seconds = 3800, DateAdded = new TimeSpan() });

            //lbOrganize.ItemsSource = videos;
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

                dbService.AddVideo(fileName, path, fileSize.ToString(), fileResolution,
                        fileFps, fileDuration.ToString(), "");
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
            //TODO: Doubleclick functionality on list view
            throw new NotImplementedException();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Edit button click
            Logger.LogText = "appending";

        }
    }
}
