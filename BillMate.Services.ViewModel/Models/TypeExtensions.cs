using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillMate.Services.ViewModel.Models
{
    public static class TypeExtensions
    {
        #region Double extentions
        //Former formatCurrency
        /// <summary>
        /// Takes a double and formats it into a string with a beginning $ and two decimal places. (e.g. $12.09)
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static string ToCurrencyString(this double currency)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:C}", currency);
        }
        #endregion

        #region String Extensions
        //Former formatDec
        public static string ToTwoDecimal(this string format)
        {
            format = format.Contains(".") == false ? format += ".00" : format;
            return string.Format("{0:F2}", format);
        }
        public static string FormatCurrencyEntry(this string content)
        {
            string value = content;
            string output = value;

            foreach (char digit in value)
            {
                if (!char.IsDigit(digit))
                    if (!char.IsPunctuation(digit))
                    {
                        var pos = value.IndexOf(digit);
                        value = value.Remove(value.IndexOf(digit));
                        output = value;
                    }

                if (digit.Equals('-'))
                {
                    value = value.Remove(value.IndexOf(digit));
                    output = value;
                }

            }

            if (value.Contains("."))
            {
                int pPos = value.IndexOf(".");
                string main = value.Substring(0, pPos);
                string dec = value.Substring(pPos + 1);

                if (dec.Contains("."))
                {
                    dec = dec.Remove(dec.IndexOf("."), 1);
                }

                if (dec.Length > 2)
                {
                    dec = dec.Substring(0, 2);
                }

                string amnt = main + "." + dec;
                output = amnt;
            }
            return output;
        }
        public static string FormatNumberEntry(this string content)
        {
            string value = content;
            string output = value;

            foreach (char digit in value)
            {
                if (!char.IsDigit(digit))
                    if (digit != '.')
                    {
                        var pos = value.IndexOf(digit);
                        value = value.Remove(pos);
                        output = value;
                    }
            }

            if (value.Contains("."))
            {
                int pPos = value.IndexOf(".");
                string main = value.Substring(0, pPos);
                string dec = value.Substring(pPos + 1);

                if (dec.Contains("."))
                {
                    dec = dec.Remove(dec.IndexOf("."), 1);
                }

                string amnt = main + "." + dec;
                output = amnt;
            }
            return output;
        }
        //Former formatEntryToNumber
        public static double ToValidDouble(this string val)
        {
            return double.Parse(string.Concat(val?.Where(c => char.IsNumber(c) || c == '.') ?? ""));
        }
        //Former extractCurrencyVal
        public static double ToCurrency(this string val)
        {
            return double.Parse(val, NumberStyles.Currency, CultureInfo.CurrentCulture);
        }
        public static string ToCurrencyString(this string val)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:C}", val);
        }
        #endregion

        #region DateTime Extensions
        /// <summary>
        /// Takes a DateTime and formats it into the following strig "dd MMM yyyy".
        /// </summary>
        /// <param name="date"></param>
        public static string ToFormattedString(this DateTime date)
        {
            return date.ToString("dd MMM yyyy");
        }
        #endregion

        #region Custom Class Extensions
        public static List<Bill> ConvertOTPtoBill(this List<OneTime> otp)
        {
            List<Bill> convList = new List<Bill>();
            int billIDGen = 9990;

            foreach (OneTime item in otp)
            {
                var convBill = new Bill();

                convBill.BillID = billIDGen;
                convBill.Name = item.Name;
                convBill.dispDueDate = item.DueDate;
                convBill.Frequency = item.Type;
                convBill.baseAmount = item.PayAmount;
                convBill.Category = item.Type;
                convBill.Reminder = "";

                convList.Add(convBill);
                billIDGen++;
            }

            return convList;
        }
        #endregion
    }
}
