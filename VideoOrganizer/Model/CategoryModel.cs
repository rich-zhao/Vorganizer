using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOrganizer.Model
{
    public class CategoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CategoryModel(long Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public CategoryModel()
        {
        }
    }
}
