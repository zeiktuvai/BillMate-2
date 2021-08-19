using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using BillMate.Services.ViewModel.Providers;
using BillMate.Services.ViewModel.Models;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Dialog
{
    public sealed partial class editBill : ContentDialog
    {       
        Bill _selectedBill;
        public List<String> Days = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        public BillDataProvider BP { get; set; } = new BillDataProvider();

        public editBill(Bill bill)
        {
            this.InitializeComponent();     
            _selectedBill = bill;
            tbxName.Text = _selectedBill.Name;
            tbxCat.Text = _selectedBill.Category;
            tbxAmnt.Text = _selectedBill.baseAmount.ToString();
            switch (_selectedBill.Frequency)
            {
                case "Monthly":
                    tbxDue.Date = DateTime.Parse(_selectedBill.dispDueDate);
                    cbxFreq.SelectedIndex = 0;
                    tbxDue.MonthVisible = false;
                    tbxDue.YearVisible = false;
                    tbxDue.Visibility = Visibility.Visible;
                    tbxDue2.Visibility = Visibility.Collapsed;
                    cbxDayDue.Visibility = Visibility.Collapsed;
                    break;
                case "Bi-Monthly":
                    var date = _selectedBill.dispDueDate.Split(',');
                    tbxDue.Date = DateTime.Parse(date[0]);
                    tbxDue2.Date = DateTime.Parse(date[1]);
                    cbxFreq.SelectedIndex = 1;
                    tbxDue.Visibility = Visibility.Visible;
                    tbxDue2.Visibility = Visibility.Visible;
                    cbxDayDue.Visibility = Visibility.Collapsed;
                    break;
                case "Weekly":
                    cbxDayDue.SelectedIndex = Days.FindIndex(d => d == _selectedBill.dispDueDate);
                    cbxFreq.SelectedIndex = 2;
                    tbxDue.Visibility = Visibility.Collapsed;
                    tbxDue2.Visibility = Visibility.Collapsed;
                    cbxDayDue.Visibility = Visibility.Visible;
                    break;
                case "Annual":
                    tbxDue.Date = DateTime.Parse(_selectedBill.dispDueDate);
                    cbxFreq.SelectedIndex = 3;
                    tbxDue.MonthVisible = true;
                    tbxDue.YearVisible = true;
                    tbxDue.MonthFormat = "{month.full}";
                    tbxDue.Visibility = Visibility.Visible;
                    tbxDue2.Visibility = Visibility.Collapsed;
                    cbxDayDue.Visibility = Visibility.Collapsed;
                    break;
            }
            this.Title = "Edit " + _selectedBill.Name;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _selectedBill.Name = tbxName.Text;
            _selectedBill.baseAmount = double.Parse(tbxAmnt.Text.TrimStart('$'));
            _selectedBill.Frequency = (cbxFreq.SelectedValue as ComboBoxItem).Content.ToString();

            switch (_selectedBill.Frequency)
            {
                case "Monthly":
                    _selectedBill.dispDueDate = tbxDue.Date.ToString();
                    break;
                case "Bi-Monthly":
                    _selectedBill.dispDueDate = (tbxDue.Date.ToString() + "," + tbxDue2.Date.ToString());
                    break;
                case "Weekly":
                    _selectedBill.dispDueDate = (cbxDayDue.SelectedValue as ComboBoxItem).Content.ToString();
                    break;
                case "Annual":
                    _selectedBill.dispDueDate = tbxDue.Date.ToString();
                    break;
            }

            _selectedBill.Category = tbxCat.Text;
            BP.UpdateBill(_selectedBill);            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void tbxAmnt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            tbxAmnt.Text = tbxAmnt.Text.FormatCurrencyEntry();
            tbxAmnt.SelectionStart = tbxAmnt.Text.Length;
        }

        private void cbxFreq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((cbxFreq.SelectedValue as ComboBoxItem).Content.ToString() == "Monthly")
            {
                lblDueDate.Text = "Due Date:";
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
                lblDueDate.Text = "Payment 1 Date:";
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
                lblDueDate.Text = "Day of the week:";
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
                lblDueDate.Text = "Due Date:";
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
        }

        private bool validate_entry()
        {
            var dRed = new SolidColorBrush(Windows.UI.Colors.DarkRed);
            var lGray = new SolidColorBrush(Windows.UI.Colors.LightGray);

            bool validated = true;

            if (string.IsNullOrEmpty(tbxName.Text))
            {
                tbxName.Background = dRed;
                validated = false;
            }
            else
            {
                tbxName.Background = lGray;
            }
            if (tbxAmnt.Text == "$" || string.IsNullOrEmpty(tbxAmnt.Text))
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
