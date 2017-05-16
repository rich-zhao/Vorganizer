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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //TODO: Timer this
                Dictionary<string, string> propertiesDict = new Dictionary<string, string>();
                List<string> arrHeaders = new List<string>();

                //gets the path of drag and drop file
                String[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String i in files){
                    long size = new FileInfo(i).Length;

                    var inputFile = new MediaFile {Filename = @i };
                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);
                    }
                    Console.WriteLine(propertiesDict);
                    
                }
                Console.WriteLine("test");
               // dragNdrop.Content = e.Data.GetData(DataFormats.FileDrop);
            }
        }

    }
}
