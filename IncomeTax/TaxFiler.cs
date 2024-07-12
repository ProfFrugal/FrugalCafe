using System;
using System.IO;

namespace IncomeTax
{
    public class TaxFiler
    {
        private static SocailSecurityTaxRange[] SocialSecurityTax = new SocailSecurityTaxRange[4];

        public TaxFilerClass FilerClass;
        public DateTime? Birthday1;
        public DateTime? Birthday2;

        public double Wage;
        public double InterestIncome;
        public double OrdinaryDivident;
        public double QualifiedDivident;
        public double ShortTermCapitalGain;
        public double LongTermCapitalGain;
        public double SocialSecurityBenefit;
        public double IRAWithdrawl;

        public double TotalIncome => OrdinaryIncome + LongTermIncome + SocialSecurityBenefit;

        public double OrdinaryIncome => Wage + InterestIncome + OrdinaryDivident + ShortTermCapitalGain + IRAWithdrawl;

        public double LongTermIncome => LongTermCapitalGain + QualifiedDivident;

        public double GetTax(int year, out double ordinalRate, TextWriter writer)
        {
            if (!TaxYear.TaxYears.TryGetValue(year, out TaxYear taxYear))
            {
                throw new ArgumentOutOfRangeException(nameof(year));
            }

            double ordinary = Wage + InterestIncome + OrdinaryDivident + ShortTermCapitalGain + IRAWithdrawl;
            double longterm = LongTermCapitalGain + QualifiedDivident;

            double socialSecurityRate = 0;

            if (SocialSecurityBenefit > 0)
            {
                double modifiedAGI = ordinary + longterm + SocialSecurityBenefit / 2;

                double taxableSocialSecurity = SocialSecurityTax[(int)FilerClass].GetTaxable(modifiedAGI, SocialSecurityBenefit);

                socialSecurityRate = taxableSocialSecurity / SocialSecurityBenefit;

                ordinary += taxableSocialSecurity;
            }

            double deduction = taxYear.OrdinalIncome.GetDeduction(year, this);

            ordinary -= deduction;

            double tax = taxYear.OrdinalIncome.GetTax(ordinary, this, year, out ordinalRate) + 
                   taxYear.LongTermCapitcalGain.GetTax(longterm, this, year, out double longTermRate);

            if (writer != null)
            {
                writer.WriteLine("Ordinary income: ${0:N2}", OrdinaryIncome);
                writer.WriteLine("LongTerm income: ${0:N2}", LongTermIncome);
                writer.WriteLine("SocialSe income: ${0:N2}", SocialSecurityBenefit);
                writer.WriteLine("401K withdrawl : ${0:N2}", IRAWithdrawl);
                writer.WriteLine("Total income   : ${0:N2}", TotalIncome);
                writer.WriteLine();

                writer.WriteLine("Federal Tax:  ${0:N2}", tax);
                writer.WriteLine("Margin Rate:  {0:P2}", tax / TotalIncome);
                writer.WriteLine("Ordinary bracket:  {0:P2}", ordinalRate);
                writer.WriteLine("LongTerm bracket:  {0:P2}", longTermRate);
                writer.WriteLine("SocialSe taxable:  {0:P2}", socialSecurityRate);
            }

            return tax;
        }

        public double MaximumIRAWithdrawl(int year, double marginRate)
        {
            double min = 0;
            double max = 200 * 1000;

            do
            {
                double mid = (min + max) / 2;

                IRAWithdrawl = mid;

                GetTax(year, out double ordinalRate, null);

                if (ordinalRate > marginRate)
                {
                    max = mid;
                    continue;
                }
                else
                {
                    min = mid;
                }

                if ((max - min) < 0.01)
                {
                    break;
                }
            }
            while (true);

            return min;
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
