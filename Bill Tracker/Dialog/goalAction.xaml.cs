using BillMate.Services.ViewModel.Models;
using BillMate.Services.ViewModel.Providers;
using Windows.UI.Xaml.Controls;

namespace Bill_Tracker.Dialog
{
    public sealed partial class goalAction : ContentDialog
    {
        Goal _goal = new Goal();
        bool _sub;
        internal GoalDataProvider GP { get; set; } = new GoalDataProvider();
        public goalAction(Goal aGoal, bool subtract)
        {
            this.InitializeComponent();
            _goal = aGoal;
            _sub = subtract;

            if(subtract == true)
            {
                this.Title = "Withdraw Funds";
            }
            else
            {
                this.Title = "Add Funds";
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (tbxAmmnt.Text.Length > 1)
            {
                if (_sub == true)
                {
                    if (tbxAmmnt.Text.ToCurrency() > _goal.currentAmount)
                    {
                        _goal.currentAmount = 0;
                    }
                    else
                    {
                        _goal.currentAmount = _goal.currentAmount - tbxAmmnt.Text.ToCurrency();
                    }

                    if (_goal.isComplete == true)
                    {
                        _goal.isComplete = false;
                    }
                }
                else
                {
                    if ((tbxAmmnt.Text.ToCurrency() + _goal.currentAmount) >= _goal.goalAmount)
                    {
                        _goal.currentAmount = _goal.goalAmount;
                        _goal.isComplete = true;

                    }
                    else
                    {
                        _goal.currentAmount = _goal.currentAmount + tbxAmmnt.Text.ToCurrency();
                        _goal.isComplete = false;
                    }
                }       
            }
            GP.UpdateGoal(_goal);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void tbxAmmnt_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.FormatCurrencyEntry();
            (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
        }
    }
}
