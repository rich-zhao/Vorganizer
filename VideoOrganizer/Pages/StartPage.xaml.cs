using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoOrganizer
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void btnNewDatabase_Click(object sender, RoutedEventArgs e)
        {
            //generate new SQL database
            this.NavigationService.Navigate(new MainPage());
        }

        private void btnLoadDatabase_Click(object sender, RoutedEventArgs e)
        {
            //TODO: load existing sql database and push to mainpage
            this.NavigationService.Navigate(new MainPage(null));
        }
    }
}
