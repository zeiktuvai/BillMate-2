using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Dialog
{
    public sealed partial class addBill : ContentDialog
    {
        Frame _mainframe;
        OneTime _otp;
        bool isOTP = false;
        bool isEdit = false;
        public BillDataProvider BP { get; set; } = new BillDataProvider();
        public OTPDataProvider OTP { get; set; } = new OTPDataProvider();

        public addBill(Frame mainframe)
        {
            this.InitializeComponent();        
            _mainframe = mainframe;
            cbxFreq.SelectedIndex = 0;
            IsPrimaryButtonEnabled = false;
        }

        public addBill(Frame mainframe, bool AddOTP)
        {
            this.InitializeComponent();
            _mainframe = mainframe;
            cbxFreq.SelectedIndex = 4;
            IsPrimaryButtonEnabled = false;
            this.Title = "Add One-Time Payment";
            tbxDue.Date = DateTime.Now.Date;
        }

        public addBill(OneTime otpToEdit)
        {
            this.InitializeComponent();
            isOTP = true;
            isEdit = true;
            _otp = otpToEdit;
            tbxName.Text = _otp.Name;
            tbxDue.Date = DateTime.Parse(_otp.DueDate);
            tbxAmnt.Text = _otp.PayAmount.ToString();
            this.Title = "Edit " + _otp.Name;
            cbxFreq.SelectedIndex = 4;

            lblDueDate.Text = "Date Due:";
            lblDueDate.Visibility = Visibility.Visible;
            tbxDue.Visibility = Visibility.Visible;
            lblDueDate2.Visibility = Visibility.Collapsed;
            tbxDue2.Visibility = Visibility.Collapsed;
            lblDueDay.Visibility = Visibility.Collapsed;
            cbxDayDue.Visibility = Visibility.Collapsed;
            tbxDue.MonthVisible = true;
            tbxDue.YearVisible = true;
            tbxCat.Visibility = Visibility.Collapsed;
            lblCat.Visibility = Visibility.Collapsed;
            PrimaryButtonText = "Edit";
            cbxFreq.IsEnabled = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {           
            if (isEdit == false)
            {
            Bill cBill = new Bill();
            OneTime cOTP = new OneTime();
                if (validate_entry())
                {
                    cBill.Name = tbxName.Text;
                    cBill.Frequency = (cbxFreq.SelectedValue as ComboBoxItem).Content.ToString();
                    switch (cBill.Frequency)
                    {
                        case "Monthly":
                            cBill.dispDueDate = tbxDue.Date.ToString();
                            cBill.Reminder = "false";
                            break;
                        case "Bi-Monthly":
                            cBill.dispDueDate = (tbxDue.Date.ToString() + "," + tbxDue2.Date.ToString());
                            cBill.Reminder = "false";
                            break;
                        case "Weekly":
                            cBill.dispDueDate = (cbxDayDue.SelectedValue as ComboBoxItem).Content.ToString();
                            cBill.Reminder = "false";
                            break;
                        case "Annual":
                            cBill.dispDueDate = tbxDue.Date.ToString();
                            cBill.Reminder = "Future";
                            break;
                        case "One Time Payment":
                            cOTP.Name = tbxName.Text;
                            cOTP.DueDate = tbxDue.Date.ToString();
                            cOTP.Paid = 0;
                            isOTP = true;
                            cOTP.Type = "OneTime";
                            break;
                    }

                    if (isOTP == false)
                    {
                        cBill.baseAmount = double.Parse(tbxAmnt.Text.TrimStart('$'));

                        if (tbxCat.Text == "")
                        {
                            cBill.Category = "Bill";
                        }
                        else
                        {
                            cBill.Category = tbxCat.Text;
                        }

                        BP.CreateBill(cBill);                        
                                                
                        if (_mainframe.Content.ToString() == "Bill_Tracker.payHistory")
                        {
                            _mainframe.Navigate(typeof(BillManagement));
                        } else
                        {
                            _mainframe.Navigate(typeof(billList));
                        }
                    } else
                    {
                        cOTP.PayAmount = double.Parse(tbxAmnt.Text.TrimStart('$'));                        
                        OTP.CreateOTP(cOTP);
                        this.Hide();
                    }             
                }    
            }
            else
            {
                if (validate_entry())
                {
                    _otp.Name = tbxName.Text;
                    _otp.PayAmount = tbxAmnt.Text.ToCurrency();
                    _otp.DueDate = tbxDue.Date.ToString();
                    OTP.UpdateOTP(_otp);
                    this.Hide();
                }
            }
        }



        private bool validate_entry()
        {
            var dRed = new SolidColorBrush(Windows.UI.Colors.DarkRed);
            var lGray = new SolidColorBrush(Windows.UI.Colors.LightGray);

            bool validated = true;

            if (tbxName.Text == "" || tbxName.Text == null)
            {
                tbxName.Background = dRed;
                validated = false;
            }
            else
            {
                tbxName.Background = lGray;
            }
            if (tbxAmnt.Text == "" || tbxAmnt.Text == null)
            {
                tbxAmnt.Background = dRed;
                validated = false;
            }
            else
            {
                tbxAmnt.Background = lGray;
            }

            return validated;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void cbxFreq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((cbxFreq.SelectedValue as ComboBoxItem).Content.ToString() == "Monthly")
            {
                lblDueDate.Text = "Day of the Month Due:";
                lblDueDate.Visibility = Visibility.Visible;
                tbxDue.Visibility = Visibility.Visible;
                lblDueDate2.Visibility = Visibility.Collapsed;
                tbxDue2.Visibility = Visibility.Collapsed;
                lblDueDay.Visibility = Visibility.Collapsed;
                cbxDayDue.Visibility = Visibility.Collapsed;
                tbxDue.MonthVisible = false;
                tbxDue.YearVisible = false;
                tbxCat.Visibility = Visibility.Visible;
                lblCat.Visibility = Visibility.Visible;
            }
            else if ((cbxFreq.SelectedValue as ComboBoxItem).Content.ToString() == "Bi-Monthly")
            {
                lblDueDate.Text = "First payment Date:";
                lblDueDate.Visibility = Visibility.Visible;
                tbxDue.Visibility = Visibility.Visible;
                lblDueDate2.Visibility = Visibility.Visible;
                tbxDue2.Visibility = Visibility.Visible;
                lblDueDay.Visibility = Visibility.Collapsed;
                cbxDayDue.Visibility = Visibility.Collapsed;
                tbxDue.MonthVisible = false;
                tbxDue.YearVisible = false;
                tbxCat.Visibility = Visibility.Visible;
                lblCat.Visibility = Visibility.Visible;
            }
            else if ((cbxFreq.SelectedValue as ComboBoxItem).Content.ToString() == "Weekly")
            {
                lblDueDate.Text = "Day of the week due:";
                lblDueDate.Visibility = Visibility.Collapsed;
                tbxDue.Visibility = Visibility.Collapsed;
                lblDueDate2.Visibility = Visibility.Collapsed;
                tbxDue2.Visibility = Visibility.Collapsed;
                lblDueDay.Visibility = Visibility.Visible;
                cbxDayDue.Visibility = Visibility.Visible;
                tbxDue.MonthVisible = false;
                tbxDue.YearVisible = false;
                tbxCat.Visibility = Visibility.Visible;
                lblCat.Visibility = Visibility.Visible;
            }
            if ((cbxFreq.SelectedValue as ComboBoxItem).Content.ToString() == "Annual")
            {
                lblDueDate.Text = "Date due:";
                lblDueDate.Visibility = Visibility.Visible;
                tbxDue.Visibility = Visibility.Visible;
                lblDueDate2.Visibility = Visibility.Collapsed;
                tbxDue2.Visibility = Visibility.Collapsed;
                lblDueDay.Visibility = Visibility.Collapsed;
                cbxDayDue.Visibility = Visibility.Collapsed;
                tbxDue.MonthVisible = true;
                tbxDue.YearVisible = false;
                tbxCat.Visibility = Visibility.Visible;
                lblCat.Visibility = Visibility.Visible;
            }
            if ((cbxFreq.SelectedValue as ComboBoxItem).Content.ToString() == "One Time Payment")
            {
                lblDueDate.Text = "Date Due:";
                lblDueDate.Visibility = Visibility.Visible;
                tbxDue.Visibility = Visibility.Visible;
                lblDueDate2.Visibility = Visibility.Collapsed;
                tbxDue2.Visibility = Visibility.Collapsed;
                lblDueDay.Visibility = Visibility.Collapsed;
                cbxDayDue.Visibility = Visibility.Collapsed;
                tbxDue.MonthVisible = true;
                tbxDue.YearVisible = true;
                tbxCat.Visibility = Visibility.Collapsed;
                lblCat.Visibility = Visibility.Collapsed;
            }
        }

        private void tbxAmnt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            tbxAmnt.Text = tbxAmnt.Text.FormatCurrencyEntry();
            tbxAmnt.SelectionStart = tbxAmnt.Text.Length;
        }


        private void tbxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (validate_entry())
            {
                this.IsPrimaryButtonEnabled = true;
            }
            else
            {
                this.IsPrimaryButtonEnabled = false;
            }
        }
    }
}
