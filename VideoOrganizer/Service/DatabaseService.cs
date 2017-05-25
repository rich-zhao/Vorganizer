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

        public void OpenConnection(string path)
        {
            connection = new SQLiteConnection("Data Source=" + path + ";Version=3;");
            connection.Open();
        }
        
        public void AddVideo(string name, string path, string fileSize, string resolution, string fps, string seconds, string hash)
        {
            string sql = "INSERT INTO videos (name, path, file_size, resolution, seconds, hash) VALUES(@name, @path, @file_size, @resolution, @seconds, @hash)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@path", path);
            command.Parameters.AddWithValue("@file_size", fileSize);
            command.Parameters.AddWithValue("@resolution",resolution);
            command.Parameters.AddWithValue("@fps", fps);
            command.Parameters.AddWithValue("@seconds", seconds);
            command.Parameters.AddWithValue("@hash", hash);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Initializes a new Database according to the path the user has shown in the SaveFileDialog
        /// </summary>
        /// <param name="path">Absolute file path provided by SaveFileDialog</param>
        public void InitializeNewDb(string path)
        {
            SQLiteConnection.CreateFile(path);
            OpenConnection(path);

            string videoQuery = "CREATE TABLE videos (id integer primary key, " +
                "name text default '', " +
                "path text not null, " +
                "is_favorite integer default 0, " +
                "file_size text not null default '0', " +
                "play_count integer default 0, " +
                "rating integer default 0, " +
                "resolution text default '0x0', " +
                "fps double default 0," +
                "seconds integer default 0, " +
                "hash text, " +
                "date_added datetime default CURRENT_TIMESTAMP, " +
                "date_original datetime, " +
                "date_last_watched datetime)";
            SQLiteCommand command = new SQLiteCommand(videoQuery, connection);
            command.ExecuteNonQuery();

            string videoTagQuery = "CREATE TABLE video_tags(id integer primary key, " +
                "video_id int, " +
                "tag_id int)";
            command = new SQLiteCommand(videoTagQuery, connection);
            command.ExecuteNonQuery();

            string tagQuery = "CREATE TABLE tags(id integer primary key, " +
                "name text, " +
                "category int)";
            command = new SQLiteCommand(tagQuery, connection);
            command.ExecuteNonQuery();

            string categoriesQuery = "CREATE TABLE categories(id integer primary key, " +
                "name text)";
            command = new SQLiteCommand(categoriesQuery, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Loads an existing database from OpenFileDialog
        /// </summary>
        /// <param name="path"></param>
        public void LoadExistingDb(string path)
        {
            OpenConnection(path);

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

        /// <summary>
        /// Check if connection is open.
        /// </summary>
        /// <returns></returns>
        public Boolean IsConnectionOpen()
        {
            switch (connection.State)
            {
                case System.Data.ConnectionState.Open:
                    return true;
                    break;
                case System.Data.ConnectionState.Connecting:
                case System.Data.ConnectionState.Closed:
                default:
                    return false;

            }
        } 
            
    }
}
