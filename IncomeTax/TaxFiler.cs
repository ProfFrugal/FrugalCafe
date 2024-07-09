using System;

namespace IncomeTax
{
    public class TaxFiler
    {
        private static TaxSchedule SocialSecurityTax = new TaxSchedule();

        public TaxFilerClass FilrerClass;
        public DateTime Birthday1;
        public DateTime Birthday2;

        public double Wage;
        public double InterestIncome;
        public double OrdinaryDividens;
        public double QualifiedDividens;
        public double ShortTermCapitalGain;
        public double LongTermCapitalGain;
        public double SocialSecurity;
        public double PretaxAccountWithdrawl;

        public double GetTax(int year)
        {
            if (!TaxYear.TaxYears.TryGetValue(year, out TaxYear taxYear))
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            double ordinary = Wage + InterestIncome + OrdinaryDividens + ShortTermCapitalGain + PretaxAccountWithdrawl;
            double longterm = LongTermCapitalGain + QualifiedDividens;

            if (SocialSecurity > 0)
            {
                double incomeTest = ordinary + longterm + SocialSecurity / 2;

                if (incomeTest > 25000)
                {
                    double taxableSocialSecurity = SocialSecurity * SocialSecurityTax.GetRate(FilrerClass, incomeTest);

                    ordinary += taxableSocialSecurity;
                }
            }

            return taxYear.OrdinalIncome.GetTax(FilrerClass, ordinary) + taxYear.LongTermCapitcalGain.GetTax(FilrerClass, longterm);
        }

        static TaxFiler()
        {
            SocialSecurityTax.AddTaxBrackets(TaxFilerClass.MarriedFillingJointly, 0, new TaxBracket[]
            {
                new TaxBracket(32_000, 44_000, 0.50),
                new TaxBracket(44_000, double.MaxValue, 0.85)
            });
        }
    }
}
