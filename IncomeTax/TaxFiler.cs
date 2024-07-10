using System;

namespace IncomeTax
{
    public class TaxFiler
    {
        private static SocailSecurityTaxRange[] SocialSecurityTax = new SocailSecurityTaxRange[4];

        public TaxFilerClass FilterClass;
        public DateTime? Birthday1;
        public DateTime? Birthday2;

        public double Wage;
        public double InterestIncome;
        public double OrdinaryDividens;
        public double QualifiedDividens;
        public double ShortTermCapitalGain;
        public double LongTermCapitalGain;
        public double SocialSecurity;
        public double PretaxAccountWithdrawl;

        public double TotalIncome => OrdinaryIncome + LongTermIncome + SocialSecurity;

        public double OrdinaryIncome => Wage + InterestIncome + OrdinaryDividens + ShortTermCapitalGain + PretaxAccountWithdrawl;

        public double LongTermIncome => LongTermCapitalGain + QualifiedDividens;

        public double GetTax(int year, out double ordinalRate, out double longTermRate, out double socialSecurityRate)
        {
            if (!TaxYear.TaxYears.TryGetValue(year, out TaxYear taxYear))
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            double ordinary = Wage + InterestIncome + OrdinaryDividens + ShortTermCapitalGain + PretaxAccountWithdrawl;
            double longterm = LongTermCapitalGain + QualifiedDividens;

            socialSecurityRate = 0;

            if (SocialSecurity > 0)
            {
                double modifiedAGI = ordinary + longterm + SocialSecurity / 2;

                double taxableSocialSecurity = SocialSecurityTax[(int)FilterClass].GetTaxable(modifiedAGI, SocialSecurity);

                socialSecurityRate = taxableSocialSecurity / SocialSecurity;

                ordinary += taxableSocialSecurity;
            }

            return taxYear.OrdinalIncome.GetTax(FilterClass, ordinary, this, year, out ordinalRate) + 
                   taxYear.LongTermCapitcalGain.GetTax(FilterClass, longterm, this, year, out longTermRate);
        }

        static TaxFiler()
        {
            SocialSecurityTax[(int)TaxFilerClass.MarriedFillingJointly] = new SocailSecurityTaxRange()
            {
                Start50 = 32_000,
                Start85 = 44_000
            };
        }
    }
}
