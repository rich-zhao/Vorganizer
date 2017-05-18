using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using VideoOrganizer;
using System;

namespace VideoOrganizer
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        SaveFileDialog saveDialog;
        OpenFileDialog openDialog;
        DatabaseService dbService;
        public StartPage()
        {
            InitializeComponent();
            InitSaveDialog();
            InitLoadDialog();
            
            dbService = DatabaseService.Instance;
        }

        private void btnNewDatabase_Click(object sender, RoutedEventArgs e)
        {
            //generate new SQL database
            saveDialog.ShowDialog();
        }

        private void btnLoadDatabase_Click(object sender, RoutedEventArgs e)
        {
            //TODO: load existing sql database and push to mainpage
            //this.NavigationService.Navigate(new MainPage(null));
            openDialog.ShowDialog();
        }

        private void InitSaveDialog()
        {
            //saveDialog.CheckFileExists = true;
            //saveDialog.CheckPathExists = true;
            saveDialog = new SaveFileDialog();
            saveDialog.CreatePrompt = true;
            saveDialog.AddExtension = true;
            saveDialog.Filter = "Database | *.db";
            saveDialog.DefaultExt = "db";
            saveDialog.Title = "VideoOrganizer";
            saveDialog.FileOk += saveDialog_FileOk;
        }

        private void InitLoadDialog()
        {
            openDialog = new OpenFileDialog();
            openDialog.AddExtension = true;
            openDialog.Filter = "Database | *.db";
            openDialog.DefaultExt = "db";
            openDialog.Title = "VideoOrganizer";
            openDialog.FileOk += loadDialog_FileOk;
        }

        private void loadDialog_FileOk(object sender, CancelEventArgs e)
        {
            dbService.loadExistingDb(openDialog.FileName);
            this.NavigationService.Navigate(new MainPage());
        }

        private void saveDialog_FileOk(object sender, CancelEventArgs e)
        {
            dbService.initializeNewDb(saveDialog.FileName);
            this.NavigationService.Navigate(new MainPage());
        }

    }
}
