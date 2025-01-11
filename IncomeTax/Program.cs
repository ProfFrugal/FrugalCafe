using System;
using System.Collections.Generic;
using System.IO;

namespace IncomeTax
{
    public enum TaxKind
    {
        SocialSecurity,
        Medicare,
        AdditionalMedicareTax,

        Ordinal,
        LongTermCapitalGain,
        NetInvestmentIncome,
        MedicareIRMAA,

        State,
        City
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var f = new TaxFiler()
            {
                FilerClass = TaxFilerClass.MarriedFillingJointly,
                InterestIncome = 1_000,
                OrdinaryDivident = 1_000,
                QualifiedDivident = 10_000,
                LongTermCapitalGain = 10_000,
                IRAWithdrawl= 20_000,
                SocialSecurityBenefit = 33_000
            };

            TextWriter writer = Console.Out;

            var tax = f.GetTax(2024, out _, writer);

            double max401K = f.MaximumIRAWithdrawl(2024, 0.12);

            writer.WriteLine();
            writer.WriteLine("Maximum 401K withdrawl for {0:P2} tax bracket: ${1:N2}", 0.12, max401K);
            writer.WriteLine();

            tax = f.GetTax(2024, out _, writer);

            f.SocialSecurityBenefit = 0;

            max401K = f.MaximumIRAWithdrawl(2024, 0.12);

            writer.WriteLine();
            writer.WriteLine("No social security, maximum 401K withdrawl for {0:P2} tax bracket: ${1:N2}", 0.12, max401K);
            writer.WriteLine();

            tax = f.GetTax(2024, out _, writer);
        }
    }
}
