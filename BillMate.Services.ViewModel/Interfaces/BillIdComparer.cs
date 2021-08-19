using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.ViewModel.Interfaces
{
    public class BillIdComparer : IEqualityComparer<Bill>
    {
        public bool Equals(Bill a, Bill b)
        {
            if (int.Equals(a.BillID, b.BillID))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Bill obj)
        {
            return obj.BillID.GetHashCode();
        }
    }
}
