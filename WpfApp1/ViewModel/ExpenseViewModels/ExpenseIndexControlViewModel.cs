using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WpfApp1.Helper;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.ViewModel.ExpenseViewModels
{
    public class ExpenseIndexControlViewModel : ReactiveObject
    {
        private string _description;
        private double _amount;
        private readonly ExpenseRepo _expenseRepo;
        private readonly Interaction<Unit, string> filePath;

        public Interaction<Unit, string> FilePath => filePath;

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public double Amount
        {
            get => _amount;
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }

        private Expense _expense;

        public Expense Expense
        {
            get => _expense;
            set => this.RaiseAndSetIfChanged(ref _expense, value);
        }

        public ReactiveList<Expense> Expenses { get; set; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; protected set; }

        private double _total;

        public double Total
        {
            get => _total;
            set => this.RaiseAndSetIfChanged(ref _total, value);
        }

        public ExpenseIndexControlViewModel()
        {
            _expenseRepo = new ExpenseRepo();
            filePath = new Interaction<Unit, string>();
            SetDefaults();
            Total = Expenses.Sum(x => x.Amount);
            var canSave = this.WhenAnyValue(x => x.Amount, x => x.Description, (amount, description)
                => !String.IsNullOrEmpty(description) && amount != 0);
            var canExport = this.WhenAnyValue(x => x.Expenses.Count, count => count > 0);
            SaveCommand = ReactiveCommand.CreateFromTask(Save, canSave);
            DeleteCommand = ReactiveCommand.CreateFromTask(Delete);
            ExportCommand = ReactiveCommand.CreateFromTask(Export, canExport);
            this.WhenAnyValue(x => x.Amount, y => y.Description).Subscribe(item =>
            {
                if (Expense == null) Expense = new Expense();
                Expense.Amount = item.Item1;
                Expense.Description = item.Item2;
            });
        }

        private void SetDefaults()
        {
            Expense = new Expense();
            Expenses = new ReactiveList<Expense>(_expenseRepo.GetDailyExpenses()) { ChangeTrackingEnabled = true };
            Expenses.ItemsAdded.Subscribe(x => Total += x.Amount);
            Expenses.ItemsRemoved.Subscribe(x => Total -= x.Amount);
        }

        private async Task Save()
        {
            if (Expense == null) return;
            if (Expense.Id != null) return;
            Expense.Id = await _expenseRepo.Add(Expense);
            Expenses.Add(Expense);
            ResetData();
        }

        private async Task Delete()
        {
            if (Expense?.Id == null) return;
            await _expenseRepo.Remove(Expense.Id);
            Expenses.Remove(Expense);
        }

        private void ResetData()
        {
            Expense = new Expense();
            Amount = 0;
            Description = String.Empty;
        }

        private async Task Export()
        {
            try
            {
                var filePath = await this.filePath.Handle(Unit.Default);
                Utility.ExportExpense(filePath, Expenses.ToList());
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}