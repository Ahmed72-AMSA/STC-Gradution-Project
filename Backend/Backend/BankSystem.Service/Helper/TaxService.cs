namespace BankSystem.Service.Helper
{
    public class TaxService
    {
        private const decimal MonthlyTaxRate = 0.01m; // 1%
        private const decimal AnnualTaxRate = 0.10m;  // 10%

        public decimal ApplyMonthlyTax(decimal balance)
        {
            if (balance < 10000) return balance; // No tax if balance is under 10K
            var tax = balance * MonthlyTaxRate;
            return balance - tax;
        }

        public decimal ApplyAnnualTax(decimal balance)
        {
            if (balance < 10000) return balance;
            var tax = balance * AnnualTaxRate;
            return balance - tax;
        }
    }
}
