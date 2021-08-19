using BillMate.Services.Data.Repository;
using BillMate.Services.ViewModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.ViewModel.Providers
{
    public class AccountDataProvider
    {
        internal SQLiteDatabaseOperation<Account> AccountProvider { get; set; }
        internal List<Trans> TransData { get; set; }

        public AccountDataProvider()
        {
            AccountProvider = new SQLiteDatabaseOperation<Account>();
            TransactionDataProvider TransProvider = new TransactionDataProvider();
            TransData = TransProvider.getTransTable().ToList();
            TransProvider = null;
        }

        public List<Account> GetAccountTable()
        {
            return formatAccount(AccountProvider.GetTable(new Account()).ToList());
        }
        public void CreateAccount(Account account)
        {
            AccountProvider.CreateItem(account);
        }
        public void UpdateAccount(Account account)
        {
            AccountProvider.UpdateItem(account);
        }
        public void DeleteAccount(Account account)
        {
            AccountProvider.DeleteItem(account);
        }

        internal List<Account> formatAccount(List<Account> acctList)
        {
            foreach (Account item in acctList)
            {
                item.dispStartDate = item.startDate.ToFormattedString();
                item.dispBal = item.acctBal.ToCurrencyString();
                item.dispMonthly = item.mnthPayAmnt.ToCurrencyString();

                if (item.acctType == AcctType.Credit)
                {
                    item.dispColor = "CornflowerBlue";
                }
                else
                {
                    item.dispColor = "LightGreen";
                }

                var transact = TransData.FindAll(t => t.accountID == item.accountID).ToList();
                var balance = transact.Sum(s => s.entryAmount);

                double payoffDays = CreditPayoffMo(item.APR, balance, item.mnthPayAmnt);
                if (!(double.IsNaN(payoffDays)) && !(double.IsInfinity(payoffDays)))
                {
                    item.estPayoff = DateTime.Now.AddMonths((int)payoffDays).ToFormattedString();
                }
                else
                {
                    item.estPayoff = "Unknown";
                }

                if (item.acctType == AcctType.Credit)
                {
                    item._currPercentage = Convert.ToInt32((Convert.ToDouble(balance) / item.creditLimit) * 100);
                    item._Limit = item.creditLimit.ToCurrencyString();
                }
                else
                {
                    item._currPercentage = Convert.ToInt32((Convert.ToDouble(balance) / item.startBal) * 100);
                    item._Limit = item.startBal.ToCurrencyString();
                }
                if (item._currPercentage > 80)
                {
                    item._radialColor = "Red";
                }
                else
                {
                    item._radialColor = "Blue";
                }
            }

            return acctList;
        }




        #region Financial Calculations
        /// <summary>
        /// Takes an APR in standard notation (e.x. 1.99) and converts it to calculateable value (.0199).
        /// </summary>
        /// <param name="APR"></param>
        internal double ConvertAPR(double APR)
        {
            double apr;
            if (APR > 100)
            {
                apr = 100 / 100;
            }
            else if (APR <= 0)
            {
                apr = .01;
            }
            else
            {
                apr = APR / 100;
            }
            return apr;
        }
        /// <summary>
        /// Takes the APR, Balance and Monthly payment of a credit card and calculates the number of months to pay it off.
        /// </summary>
        /// <param name="APR"></param>
        /// <param name="balance"></param>
        /// <param name="mPayment"></param>
        internal double CreditPayoffMo(double APR, double balance, double mPayment)
        {
            double apr = ConvertAPR(APR);

            double calculation = (Math.Log((1 + (balance / mPayment) * (1 - (Math.Pow(1 + (apr / 365), 30)))))) / (Math.Log((1 + (apr / 365))));
            double months = (-0.033333333333333333) * calculation;

            return months;
        }
        /// <summary>
        /// Takes the APR and Current Balance of an account and calculates the interest accrued for one month.
        /// </summary>
        /// <param name="APR"></param>
        /// <param name="balance"></param>        
        public double CalculateMonthlyInterest(double APR, double balance)
        {
            double aprMo = ConvertAPR(APR) / 12;
            return balance * aprMo;
        }
        /// <summary>
        /// Takes the loan APR, Monthly Payment, and Balance and returns the principal paid on the loan for a monthly payment.
        /// </summary>
        /// <param name="APR"></param>
        /// <param name="balance"></param>
        /// <param name="payment"></param>        
        public double CalculateLoanPaymentPrincipal(double APR, double balance, double payment)
        {
            return Math.Truncate((payment - CalculateMonthlyInterest(APR, balance)) * 100) / 100;
        }

        public double getAcctBal(Account account)
        {
            return TransData.FindAll(t => t.accountID == account.accountID).Sum(s => s.entryAmount);
        }
        #endregion
    }
}
