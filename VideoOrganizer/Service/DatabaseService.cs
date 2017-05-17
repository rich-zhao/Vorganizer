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

        public void initializeNewDb(string path)
        {
            
            SQLiteConnection.CreateFile(path);

        }

    }
}
