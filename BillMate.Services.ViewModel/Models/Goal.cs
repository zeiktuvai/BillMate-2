using SQLite.Net.Attributes;
using System;

namespace BillMate.Services.ViewModel.Models
{
    public class Goal
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string goalName { get; set; }
        public double goalAmount { get; set; }
        public double currentAmount { get; set; }
        public string goalDate { get; set; }
        public bool isComplete { get; set; }
        [Ignore]
        public string _completionText { get; set; }
        [Ignore]
        public double progress { get; set; }
        [Ignore]
        public string _dispCurAmnt { get; set; }
        [Ignore]
        public string _dispGoalAmnt { get; set; }
        [Ignore]
        public string _dispGoalDate { get; set; }
        [Ignore]
        public DateTime _goalDate { get; set; }
        [Ignore]
        public string _background { get; set; }
        [Ignore]
        public string _textFore { get; set; }
        [Ignore]
        public string _pastSuspense { get; set; }
        [Ignore]
        public string _barColor { get; set; }
        [Ignore]
        public string minPay { get; set; }
    }
}
