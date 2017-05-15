using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOrganizer
{
    public sealed class DatabaseService
    {
        static DatabaseService instance;
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
    }
}
