using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using VideoOrganizer.Model;
using VideoOrganizer.Service;

namespace VideoOrganizer.Windows
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryTagWindow : Window
    {
        private List<CategoryModel> categories;
        DatabaseService dbService;
        private VideoModel currVideo;

        public CategoryTagWindow()
        {
            InitializeComponent();

            dbService = DatabaseService.Instance;
            UpdateCategories();
        }

        private void UpdateCategories()
        {
            categories = dbService.FindAllCategories();
            cbCategory.ItemsSource = categories;
        }

        public CategoryTagWindow(VideoModel currVideo) : this()
        {
            this.currVideo = currVideo;
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryModel category = dbService.AddCategory(tbCategory.Text);
            UpdateCategories();
            tbCategory.Text = "";
        }

        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            dbService.AddTagToVideo(currVideo, (TagModel)cbTag.SelectedItem);
            
            this.Close();
        }

        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(((CategoryModel)cbCategory.SelectedItem).Name);
            List<TagModel> tags =dbService.FindTagsByCategory((CategoryModel)cbCategory.SelectedItem);
            cbTag.ItemsSource = tags;
            tags.ForEach(x => Debug.WriteLine(x.Tag));
        }
    }
}
