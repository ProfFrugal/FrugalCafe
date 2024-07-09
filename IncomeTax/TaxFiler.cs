using System;

namespace IncomeTax
{
    public class TaxFiler
    {
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
            if (TaxYear.TaxYears.TryGetValue(year, out TaxYear taxYear))
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            double ordinary = Wage + InterestIncome + OrdinaryDividens + ShortTermCapitalGain + PretaxAccountWithdrawl;
            double longterm = LongTermCapitalGain + QualifiedDividens;

            ordinary += SocialSecurity * 0.85;

            return taxYear.OrdinalIncome.GetTax(FilrerClass, ordinary) + taxYear.LongTermCapitcalGain.GetTax(FilrerClass, longterm);
        }

    }
}
