using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using VideoOrganizer;

namespace VideoOrganizer
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        SaveFileDialog saveDialog = new SaveFileDialog();

        public StartPage()
        {
            InitializeComponent();
            InitSaveDialog();
        }

        private void btnNewDatabase_Click(object sender, RoutedEventArgs e)
        {
            //generate new SQL database
            saveDialog.ShowDialog();
            //this.NavigationService.Navigate(new MainPage());
        }

        private void btnLoadDatabase_Click(object sender, RoutedEventArgs e)
        {
            //TODO: load existing sql database and push to mainpage
            this.NavigationService.Navigate(new MainPage(null));
        }

        private void InitSaveDialog()
        {
            //saveDialog.CheckFileExists = true;
            //saveDialog.CheckPathExists = true;
            saveDialog.CreatePrompt = true;
            saveDialog.AddExtension = true;
            saveDialog.Filter = "Database | *.db";
            saveDialog.DefaultExt = "db";
            saveDialog.Title = "VideoOrganizer";
            saveDialog.FileOk += saveDialog_FileOk;
        }

        private void saveDialog_FileOk(object sender, CancelEventArgs e)
        {
            DatabaseService dbService = DatabaseService.Instance;
            dbService.initializeNewDb(saveDialog.FileName);
        }

    }
}
