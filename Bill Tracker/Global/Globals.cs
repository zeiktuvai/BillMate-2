using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bill_Tracker.Global
{
    class Globals
    {
        public static int _WORKING_MONTH = 0;

        public static string dbPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "bills.db3");
    }
}
