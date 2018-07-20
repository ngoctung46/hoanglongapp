using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;
using WpfApp1.Repos.Base;

namespace WpfApp1.Repos
{
    public class ExpenseRepo : RepoBase<Expense>
    {
        private static List<Expense> _expenses;

        public ExpenseRepo()
        {
            Task.Run(async () =>
            {
                var expenses = await All();
                _expenses = expenses.ToList();
            }).Wait();
        }

        public async Task<IEnumerable<Expense>> All()
        {
            var expenses = await GetAsync();
            return expenses;
        }

        public async Task<String> Add(Expense expense)
        {
            var id = await PostAsync(expense);
            expense.Id = id;
            _expenses.Add(expense);
            return id;
        }

        public async Task Remove(string id)
        {
            await DeleteAsync(id);
            _expenses.RemoveAll(x => x.Id == id);
        }

        public List<Expense> GetAll() => _expenses;

        public List<Expense> GetDailyExpenses()
        {
            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var toDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            return _expenses.Where(x => x.CreatedAt.ToLocalTime() >= fromDate && x.CreatedAt.ToLocalTime() <= toDate)
                .ToList();
        }
    }
}