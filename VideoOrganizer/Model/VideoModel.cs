using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOrganizer.Model
{
    public class VideoModel
    {
        public long Id { get; set; }
        public string Name { get; internal set; }
        public string Path { get; set; }
        public bool IsFavorite { get; set; }
        public string FileSize { get; set; }
        public long PlayCount { get; set; }
        public long Rating { get; set; }
        public string Resolution { get; set; }
        public long Fps { get; set; }
        public long Minutes { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateLastWatched { get; set; }

        public VideoModel()
        {

        }

        public VideoModel(int id, string Name, string Path, bool IsFavorite, string FileSize, long PlayCount,
            long Rating, string Resolution, long Fps, long Minutes, DateTime DateAdded)
        {
            this.Name = Name;
            this.Path = Path;
            this.IsFavorite = IsFavorite;
            this.FileSize = FileSize;
            this.PlayCount = PlayCount;
            this.Rating = Rating;
            this.Resolution = Resolution;
            this.Fps = Fps;
            this.Minutes = Minutes;
            this.DateAdded = DateAdded;
        }
    }
}
