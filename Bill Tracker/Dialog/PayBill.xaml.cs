using Bill_Tracker.Global;
using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using System;
using Windows.UI.Xaml.Controls;


// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Dialog
{
    public sealed partial class PayBill : ContentDialog
    {
        public PaymentDataProvider PP { get; set; } = new PaymentDataProvider();
        public OTPDataProvider OTP { get; set; } = new OTPDataProvider();
        public string Result { get; set; }
        Bill billToPay = new Bill();        
        OneTime OTPPay = new OneTime();
        bool isOTP = false;

        public PayBill(Bill inputBill)
        {
            this.InitializeComponent();            
            billToPay = inputBill;
            Title = "Make payment to: " + billToPay.Name;
            PayDiag_tbxAmnt.Text = billToPay.baseAmount.ToString().ToTwoDecimal();
            PayDiag_dtePaid.Date = inputBill.dueDate;               
            
            if (billToPay.Frequency == "Bi-Monthly" || billToPay.Frequency == "Weekly")
            {
                PayDiag_dtePaid.Date = billToPay.dueDate;
                PayDiag_dtePaid.IsEnabled = false;
            }
        }

        public PayBill(OneTime inputOTP)
        {
            this.InitializeComponent();
            DateTime baseDate = DateTime.Now.AddMonths(Globals._WORKING_MONTH);
            OTPPay = inputOTP;
            this.Title = "Pay " + OTPPay.Name;
            PayDiag_tbxAmnt.Text = OTPPay.PayAmount.ToString().ToTwoDecimal();
            PayDiag_dtePaid.Date = baseDate;
            isOTP = true;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (isOTP == false)
            {
                var payBill = new Payment() { dispPayDate = DateTime.Parse(PayDiag_dtePaid.Date.ToString()).ToString(), amount = double.Parse(PayDiag_tbxAmnt.Text), posted = false, BillID = billToPay.BillID };
                PP.CreatePayment(payBill);               
            }
            else
            {
                OTPPay.PayAmount = double.Parse(PayDiag_tbxAmnt.Text);
                OTPPay.Paid = 1;
                OTP.UpdateOTP(OTPPay);
            }
                this.Hide();           
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = "Cancel";
            this.Hide();   
        }

        private void payDiag_tbxAmnt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string value = PayDiag_tbxAmnt.Text;

            foreach (char digit in value)
            {
                if (!char.IsDigit(digit))
                    if (!char.IsPunctuation(digit))
                    {
                        var pos = value.IndexOf(digit);
                        value = value.Remove(value.IndexOf(digit));
                        PayDiag_tbxAmnt.Text = value;
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
                PayDiag_tbxAmnt.Text = amnt;
            }
            PayDiag_tbxAmnt.SelectionStart = PayDiag_tbxAmnt.Text.Length;
        }
    }
}
