using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.Data.Interfaces
{
    public interface IDatabaseOperation<T>
    {
        IEnumerable<T> GetTable(T TableType);
        void CreateItem(T ItemToCreate);
        void UpdateItem(T ItemToUpdate);
        void DeleteItem(T ItemToDelete);
        int ExecuteCmd(string cmd);
    }
}
