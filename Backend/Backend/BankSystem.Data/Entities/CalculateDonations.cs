using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class CalculateDonations
    {
        public double MoneyValue { get; set; } = 0;
        public double PropertySharesValue { get; set; } = 0;
        public double BondsValue { get; set; } = 0;
        public double ProfitValue { get; set; } = 0;
        public double GoldValue { get; set; } = 0;
        public int GoldKarat { get; set; } = 18;
        public double BuildingValue { get; set; } = 0;
        public double DonationRate { get; set; } = 0.025;
        public double Gold18Price { get; set; } = 2259;
        public double Gold21Price { get; set; } = 2635;
    }
}
