using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoOrganizer.Model;

namespace VideoOrganizer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private DatabaseService dbService;
        private List<VideoModel> videos = new List<VideoModel>();

        public MainPage()
        {
            InitializeComponent();
            dbService = DatabaseService.Instance;
            videos = dbService.FindAllVideos();
            if (videos != null)
            {
                lvOrganize.ItemsSource = dbService.FindAllVideos();
            }

            //videos.Add(new VideoModel() { Name = "Complete this WPF tutorial", Path = "asdg", IsFavorite=true, FileSize="500", PlayCount=1, Rating=3, Resolution="1920x1080", Fps=60, Seconds=3600, DateAdded= new TimeSpan() });
            //videos.Add(new VideoModel() { Name = "Learn C#", Path = "asdg", IsFavorite = true, FileSize = "600", PlayCount = 2, Rating = 5, Resolution = "1024x768", Fps = 30, Seconds = 3300, DateAdded = new TimeSpan() });
            //videos.Add(new VideoModel() { Name = "Wash the car", Path = "asdijuh", IsFavorite = true, FileSize = "800", PlayCount = 2, Rating = 4, Resolution = "1280x1024", Fps = 120, Seconds = 3800, DateAdded = new TimeSpan() });

            //lbOrganize.ItemsSource = videos;
        }
        
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            int validFiles = 0, invalidFiles = 0;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //gets the path of drag and drop file
                //TODO: handle directory drag and drop
                String[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String i in files){
                    var inputFile = new MediaFile {Filename = @i };
                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);

                        if(inputFile.Metadata == null)
                        {
                            invalidFiles++;
                            return;
                        }
                        validFiles++;
                    }
                    long fileSize = new FileInfo(i).Length;
                    long fileFps = Convert.ToInt64(inputFile.Metadata.VideoData.Fps);
                    string fileResolution = inputFile.Metadata.VideoData.FrameSize;
                    double fileDuration = inputFile.Metadata.Duration.TotalSeconds;
                    //fileInformation = String.Format("File information: \nFile Size: {0}\n File FPS: {1}\n File Resolution: {2}\n FileDuration: {3}",
                    //    fileSize, fileFps, fileResolution, fileDuration);

                   dbService.AddVideo(i.Substring(i.LastIndexOf('\\') + 1), i, fileSize.ToString(), fileResolution,
                        fileFps, fileDuration.ToString(), "");
                    lvOrganize.ItemsSource = dbService.FindAllVideos();
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = tbSearch.Text;
            lvOrganize.ItemsSource = dbService.FindVideos(searchText);
        }

        private void tbSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                btnSearch_Click(this, new RoutedEventArgs());
            }
        }
    }
}
