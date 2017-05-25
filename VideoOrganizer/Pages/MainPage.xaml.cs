using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VideoOrganizer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private object database;

        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(object database)
        {
            this.database = database;
        }
        
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string fileInformation ="";
            int validFiles = 0, invalidFiles = 0;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //gets the path of drag and drop file
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
                    double fileFps = inputFile.Metadata.VideoData.Fps;
                    string fileResolution = inputFile.Metadata.VideoData.FrameSize;
                    double fileDuration = inputFile.Metadata.Duration.TotalSeconds;
                    //fileInformation = String.Format("File information: \nFile Size: {0}\n File FPS: {1}\n File Resolution: {2}\n FileDuration: {3}",
                    //    fileSize, fileFps, fileResolution, fileDuration);

                    DatabaseService.Instance.AddVideo(i.Substring(i.LastIndexOf('\\') + 1), i, fileSize.ToString(), fileResolution,
                        fileFps.ToString(), fileDuration.ToString(), "");
                }
                dragNdrop.Content = fileInformation;//e.Data.GetData(DataFormats.FileDrop);
            }
        }

    }
}
