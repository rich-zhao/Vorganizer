using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using VideoOrganizer.Model;

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
                "fps integer default 0," +
                "seconds integer default 0, " +
                "hash text, " +
                "date_added datetime default CURRENT_TIMESTAMP, " +
                "date_original datetime, " +
                "date_last_watched datetime)";
            SQLiteCommand command = new SQLiteCommand(videoQuery, connection);
            command.ExecuteNonQuery();

            string videoTagQuery = "CREATE TABLE video_tags(id integer primary key, " +
                "video_id integer, " +
                "tag_id integer)";
            command = new SQLiteCommand(videoTagQuery, connection);
            command.ExecuteNonQuery();

            string tagQuery = "CREATE TABLE tags(id integer primary key, " +
                "name text, " +
                "category integer)";
            command = new SQLiteCommand(tagQuery, connection);
            command.ExecuteNonQuery();

            string categoriesQuery = "CREATE TABLE categories(id integer primary key, " +
                "name text)";
            command = new SQLiteCommand(categoriesQuery, connection);
            command.ExecuteNonQuery();
        }

        public void AddVideo(string name, string path, string fileSize, string resolution, long fps, string seconds, string hash, DateTime date_original, DateTime date_last_watched)
        {
            string sql = "INSERT INTO videos (name, path, file_size, resolution, fps, seconds, hash, date_original, date_last_watched) VALUES(@name, @path, @file_size, @resolution,@fps, @seconds, @hash, @date_original, @date_last_watched)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@path", path);
            command.Parameters.AddWithValue("@file_size", fileSize);
            command.Parameters.AddWithValue("@resolution",resolution);
            command.Parameters.AddWithValue("@fps", fps);
            command.Parameters.AddWithValue("@seconds", seconds);
            command.Parameters.AddWithValue("@hash", hash);
            command.Parameters.AddWithValue("@date_original", date_original);
            command.Parameters.AddWithValue("@date_last_watched", date_last_watched);

            command.ExecuteNonQuery();
        }

        public void AddVideo(VideoModel video)
        {
            string sql = "INSERT INTO videos (name, path, file_size, resolution, fps, seconds, hash, date_original, date_last_watched) VALUES(@name, @path, @file_size, @resolution,@fps, @seconds, @hash, @date_original, @date_last_watched)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", video.Name);
            command.Parameters.AddWithValue("@path", video.Path);
            command.Parameters.AddWithValue("@file_size", video.FileSize);
            command.Parameters.AddWithValue("@resolution", video.Resolution);
            command.Parameters.AddWithValue("@fps", video.Fps);
            command.Parameters.AddWithValue("@seconds", video.Minutes);
            command.Parameters.AddWithValue("@hash", "");
            command.Parameters.AddWithValue("@date_original", video.DateOriginal);
            command.Parameters.AddWithValue("@date_last_watched", video.DateLastWatched);

            command.ExecuteNonQuery();
        }

        public CategoryModel AddCategory(string name)
        {
            name = name.Trim();
            //check if already exists
            CategoryModel category = FindAllCategories().Find(cat => cat.Name.Equals(name));
            if (category != null) return category;

            //if not exist then add
            string sql = "INSERT INTO categories(name) VALUES(@name)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);

            command.ExecuteNonQuery();

            return FindCategoryByName(name);
        }

        public void AddTag(string name, string categoryName)
        {
            name = name.Trim();

            //sql get category by name
            //if category exists, get category id and use it to insert new tag
            //add Tag

            CategoryModel category = FindCategoryByName(categoryName);
            if (category == null) return;

            string sql = "INSERT INTO tags(name, category) VALUES(@name, @category)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@category", category.Id);

            command.ExecuteNonQuery();
        }
        
        public void AddTagToVideo(VideoModel video, TagModel tag)
        {
            if(tag != null && video != null)
                AddTagToVideo(video.Id, tag.Id);
        }

        public void AddTagToVideo(long videoId, long tagId)
        {
            //check to see if tag already exists in video
            if(FindVideoTags(videoId).Exists(tag => tag.Id.Equals(tagId))) return;

            //if not add tag to video
            string sql = "INSERT INTO video_tags(video_id, tag_id) VALUES (@videoId, @tagId)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@videoId", videoId);
            command.Parameters.AddWithValue("@tagId", tagId);

            command.ExecuteNonQuery();
        }

        public List<TagModel> FindVideoTags(VideoModel video)
        {
            return FindVideoTags(video.Id);
        }

        public List<TagModel> FindVideoTags(long videoId)
        {
            List<TagModel> videoTags = new List<TagModel>();

            string sql = "SELECT * FROM video_tags WHERE video_id = @videoId";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@videoId", videoId);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
               videoTags.Add(FindTag((long)reader["tag_id"]));
            }

            return videoTags;
        }

        public TagModel FindTag(long tagId)
        {
            string sql = "SELECT * FROM tags WHERE id = @tagId";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@tagId", tagId);
            SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();
            return new TagModel((long)reader["id"], (string)reader["name"], (long)reader["category"]);
            
        }

        public TagModel FindTagByName(string tagName)
        {
            string sql = "SELECT * FROM tags WHERE name = @tagName";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@tagName", tagName);
            SQLiteDataReader reader = command.ExecuteReader();

            reader.Read();
            return new TagModel((long)reader["id"], (string)reader["name"], (long)reader["category"]);

        }

        public List<TagModel> FindTagsByCategory(CategoryModel category)
        {
            return FindTagsByCategory(category.Id);
        }

        public List<TagModel> FindTagsByCategory(long categoryId)
        {
            List<TagModel> tags = new List<TagModel>();
            string sql = "SELECT * FROM tags WHERE category = @categoryId";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@categoryId", categoryId);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read()) {
                TagModel model = new TagModel((long)reader["id"], (string)reader["name"], (long)reader["category"]);
                tags.Add(model);
            }
            return tags;
        }

        public CategoryModel FindCategory(long categoryId)
        {
            string sql = "SELECT * FROM categories WHERE id = @categoryId";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@categoryId", categoryId);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                return new CategoryModel((long)reader["id"],(string)reader["name"]);
            }
            return null;
        }

        public CategoryModel FindCategoryByName(string categoryName)
        {
            string sql = "SELECT * FROM categories WHERE name = @categoryName";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@categoryName", categoryName);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                return new CategoryModel((long)reader["id"], (string)reader["name"]);
            }
            return null;
        }

        public List<CategoryModel> FindAllCategories()
        {
            if (!IsConnectionOpen()) return null;
            List<CategoryModel> categories = new List<CategoryModel>();

            string sql = "SELECT * FROM categories";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                CategoryModel c = new CategoryModel();
                c.Id = (long) reader["id"];
                c.Name = (string)reader["name"];

                categories.Add(c);
            }

            return categories;
        }

        //Finds All videos from the database
        public List<VideoModel> FindAllVideos()
        {
            if (!IsConnectionOpen()) return null;
            List<VideoModel> videos = new List<VideoModel>();

            string getVideoQuery = "SELECT * FROM Videos";
            SQLiteCommand command = new SQLiteCommand(getVideoQuery, connection);
            SQLiteDataReader reader = command.ExecuteReader();

            ParseVideos(reader, videos);

            return videos;
        }

        public List<VideoModel> FindVideos(string fileName)
        {
            if (!IsConnectionOpen()) throw new Exception();
            List<VideoModel> videos = new List<VideoModel>();

            string getVideoQuery = "SELECT * FROM Videos WHERE name LIKE @name";
            SQLiteCommand command = new SQLiteCommand(getVideoQuery, connection);
            command.Parameters.AddWithValue("@name", "%" + fileName + "%");
            SQLiteDataReader reader = command.ExecuteReader();

            ParseVideos(reader, videos);
            
            return videos;
        }

        /// <summary>
        /// Deletes Video from the database and its associated records in other tables
        /// </summary>
        public void DeleteVideo(VideoModel video)
        {
            DeleteVideo(video.Id);

        }
        public void DeleteVideo(long videoId)
        {
            string sql = "DELETE FROM videos WHERE id = @videoId; " +
                "DELETE FROM video_tags WHERE video_id = @videoId";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@videoId", videoId);
            command.ExecuteNonQuery();

        }

        public void UpdateVideo(VideoModel video)
        {
            if (!IsConnectionOpen()) throw new Exception();
            string updateVideoQuery = "UPDATE Videos SET is_favorite=@favorite, play_count=@play_count, " +
                "rating=@rating, date_last_watched=@date_last_watched WHERE id =@id";
            SQLiteCommand command = new SQLiteCommand(updateVideoQuery, connection);
            command.Parameters.AddWithValue("@id", video.Id);
            command.Parameters.AddWithValue("@favorite", video.IsFavorite);
            command.Parameters.AddWithValue("@play_count", video.PlayCount);
            command.Parameters.AddWithValue("@rating", video.Rating);
            command.Parameters.AddWithValue("@date_last_watched", video.DateLastWatched);
            command.ExecuteNonQuery();
        }

        private void ParseVideos(SQLiteDataReader reader, List<VideoModel> videos)
        {
            while (reader.Read())
            {
                VideoModel video = new VideoModel();
                video.Name = (string)reader["name"];
                video.Path = (string)reader["path"];
                if (((long)reader["is_favorite"]) == 0)
                {
                    video.IsFavorite = false;
                }
                else
                {
                    video.IsFavorite = true;
                }
                video.Id = (long)reader["id"];
                video.FileSize = (string)reader["file_size"];
                video.PlayCount = (long)reader["play_count"];
                video.Rating = (long)reader["rating"];
                video.Resolution = (string)reader["resolution"];
                video.Fps = (long)reader["fps"];
                video.Minutes = (long)reader["seconds"] / 60;
                video.DateAdded = (DateTime)reader["date_added"];
                if (!reader.IsDBNull(reader.GetOrdinal("date_original")))
                    video.DateOriginal = (DateTime)reader["date_original"];
                if (!reader.IsDBNull(reader.GetOrdinal("date_last_watched")))
                    video.DateLastWatched = (DateTime)reader["date_last_watched"];
                videos.Add(video);
            }
        }

        public void DeleteTagFromVideo(VideoModel video, TagModel tag)
        {
            string sql = "DELETE FROM video_tags WHERE video_id = @videoId AND tag_id = @tagId";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@videoId", video.Id);
            command.Parameters.AddWithValue("@tagId", tag.Id);
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
