using System;
using System.Collections.Generic;
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
                Shell32.Shell shell = new Shell32.Shell();
                Shell32.Folder objFolder;

                //gets the path of drag and drop file
                String[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String i in files){
                    string file = i.Split('\\').Last();
                    string path = i.Replace(file, "");
              
                    objFolder = shell.NameSpace(@path);
                    Shell32.ShellFolderItem item = (Shell32.ShellFolderItem)objFolder.Items().Item(file);
                    string prop = item.ExtendedProperty("{64440491-4C8B-11D1-8B70-080036B11A03}");
                    /*
                    //gets extended properties' headers
                    for (int j=0; j<short.MaxValue; j++)
                    {
                        string header = objFolder.GetDetailsOf(null, j);
                        if (String.IsNullOrEmpty(header))
                            continue;
                        arrHeaders.Add(header);
                        propertiesDict[header] = "";
                    }

                    //set actual file properties
                    for(int j=0; j<arrHeaders.Count; j++)
                    {
                        string header = arrHeaders[j];
                        string value = objFolder.GetDetailsOf(objFolder.ParseName(file), j);
                        propertiesDict[arrHeaders[j]] = objFolder.GetDetailsOf(objFolder.ParseName(file), j);
                    }
                    */
                    Console.WriteLine(propertiesDict);
                    
                }
                //DirectoryInfo di = new DirectoryInfo();
                //FileInfo fi = new FileInfo();
                Console.WriteLine("test");
               // dragNdrop.Content = e.Data.GetData(DataFormats.FileDrop);
            }
        }

    }
}
