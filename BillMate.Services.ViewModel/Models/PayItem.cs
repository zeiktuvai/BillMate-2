using System.Collections.Generic;

namespace BillMate.Services.ViewModel.Models
{
    public class PayItem
    {
        public string name { get; set; }
        public double amount { get; set; }
        public double remain { get; set; }
        public string _foreColor { get; set; }
        public string _amount { get; set; }
        public string _remain { get; set; }
        public bool _paid { get; set; }
        public string _gridBG { get; set; }

        //TODO: extract to provider
        public static List<PayItem> ProcessPayItems(List<PayItem> pays, double startBal)
        {
            double runningBal = startBal;
            foreach (var item in pays)
            {
                item.remain = runningBal - item.amount;
                item._remain = item.remain.ToCurrencyString();
                runningBal = runningBal - item.amount;
                item._amount = item.amount.ToCurrencyString();
                if (item.remain > 0)
                {
                    item._foreColor = "Green";
                }
                else
                {
                    item._foreColor = "Red";
                }
                if (item._paid == true)
                {
                    item._gridBG = "#FFBBE1BB";
                }
                else
                {
                    item._gridBG = "White";
                }
            }
            return pays;
        }
    }
}
