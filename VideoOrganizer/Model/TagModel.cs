using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOrganizer.Model
{
    public class TagModel
    {
        public long Id { get; set; }
        public string Tag { get; set; }
        public CategoryModel Category { get; set; }

        public TagModel()
        {

        }

        public TagModel(long Id, string Tag, CategoryModel Category)
        {
            this.Id = Id;
            this.Tag = Tag;
            this.Category = Category;
        }

        public TagModel(long Id, string Tag, long CategoryId)
        {
            this.Id = Id;
            this.Tag = Tag;
            this.Category = DatabaseService.Instance.FindCategory(CategoryId);
        }
    }
}
