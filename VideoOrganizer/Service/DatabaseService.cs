using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace VideoOrganizer
{
    public class DatabaseService
    {
        private static DatabaseService instance;
        private static SQLiteConnection connection;

        private DatabaseService() { }

        public static DatabaseService Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DatabaseService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes a new Database according to the path the user has shown in the SaveFileDialog
        /// </summary>
        /// <param name="path">Absolute file path provided by SaveFileDialog</param>
        public void initializeNewDb(string path)
        {
            SQLiteConnection.CreateFile(path);
            connection = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            connection.Open();
            string videoQuery = "CREATE TABLE videos (id int, name text, path text, is_favorite integer, " +
                "file_size text, play_count integer, rating integer, resolution text, seconds integer, " +
                "hash text, date_added datetime, date_original datetime, date_last_watched datetime)";
            SQLiteCommand command = new SQLiteCommand(videoQuery, connection);
            command.ExecuteNonQuery();

            string videoTagQuery = "CREATE TABLE video_tags(id int, video_id int, tag_id int)";
            command = new SQLiteCommand(videoTagQuery, connection);
            command.ExecuteNonQuery();

            string tagQuery = "CREATE TABLE tags(id int, name text, category int)";
            command = new SQLiteCommand(tagQuery, connection);
            command.ExecuteNonQuery();

            string categoriesQuery = "CREATE TABLE categories(id int, name text)";
            command = new SQLiteCommand(categoriesQuery, connection);
            command.ExecuteNonQuery();
        }

        public void loadExistingDb(string path)
        {
            connection = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            connection.Open();

            //testing proof of concept. remove later
            string s = "SELECT * FROM categories";
            string total = "";

            SQLiteCommand command = new SQLiteCommand(s, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string r = reader["id"] + " " +  reader["name"] + "\n";
                total += r;
            }

        }

    }
}
