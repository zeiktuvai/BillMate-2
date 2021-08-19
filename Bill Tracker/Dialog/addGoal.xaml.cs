using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Bill_Tracker.Dialog
{
    public sealed partial class addGoal : ContentDialog
    {
        public Goal _goal { get; set; }
        public bool _isEdit { get; set; }
        internal GoalDataProvider GP { get; set; } = new GoalDataProvider();
        public addGoal(bool isEdit = false, Goal eGoal = null)
        {
            this.InitializeComponent();
            _goal = eGoal;
            _isEdit = isEdit;
            if(isEdit == true)
            {
                this.Title = string.Format("Edit {0}", eGoal.goalName);
                tbxGoalName.Text = eGoal.goalName;
                tbxAmnt.Text = eGoal.goalAmount.ToString();
                tbxCurAmnt.Text = eGoal.currentAmount.ToString();
                dteDate.Date = eGoal._goalDate;
                tbxCurAmnt.Visibility = Visibility.Visible;
                lblCurAmnt.Visibility = Visibility.Visible;
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
                if (_goal != null && _isEdit == true)
                {
                    Goal uac = _goal;
                    uac.currentAmount = tbxCurAmnt.Text.ToCurrency();
                    uac.goalAmount = tbxAmnt.Text.ToCurrency();
                    uac.goalName = tbxGoalName.Text;
                    uac.goalDate = dteDate.Date.ToString();                
                    uac.isComplete = false;
                
                    GP.UpdateGoal(uac);
                
                }
                else
                {
                    if (tbxAmnt.Text.Length != 0 && tbxGoalName.Text.Length != 0)
                    {
                        Goal nGoal = new Goal() { goalAmount = tbxAmnt.Text.ToCurrency(), goalName = tbxGoalName.Text, currentAmount = 0, goalDate = dteDate.Date.ToString(), isComplete = false };
                        GP.CreateGoal(nGoal);
                    }
                }            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void tbxAmnt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.ToCurrencyString();
            (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
            if ((sender as TextBox).Text.Length <= 1)
            {
                IsPrimaryButtonEnabled = false;
            } else { IsPrimaryButtonEnabled = true;  }
        }

        private void tbxGoalName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length <= 1)
            {
                IsPrimaryButtonEnabled = false;
            }
            else { IsPrimaryButtonEnabled = true; }
        }

        private void tbxAmnt_Tapped(object sender, TappedRoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
