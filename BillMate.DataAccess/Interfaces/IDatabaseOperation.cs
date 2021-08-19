using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.DataAccess.Interfaces
{
    public interface IDatabaseOperation
    {
        string dbPath { get; }
        SQLiteConnection con { get; set; }

        void openCon();
        void closeCon();
        void createItem();
        void updateItem();
        void deleteItem();
    }
}
