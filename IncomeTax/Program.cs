using System;
using System.Collections.Generic;
using System.IO;

namespace IncomeTax
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var f = new TaxFiler()
            {
                FilterClass = TaxFilerClass.MarriedFillingJointly,
                InterestIncome = 5_000,
                OrdinaryDivident = 5_000,
                QualifiedDivident = 10_000,
                PretaxAccountWithdrawl= 30_000,
                SocialSecurity = 30_000
            };

            var tax = f.GetTax(2024, out double ordinalRate, out double longTermRate, out double socialSecurityTaxable);

            TextWriter writer = Console.Out;

            writer.WriteLine("Ordinary income: ${0:N2}", f.OrdinaryIncome);
            writer.WriteLine("LongTerm income: ${0:N2}", f.LongTermIncome);
            writer.WriteLine("SocialSe income: ${0:N2}", f.SocialSecurity);
            writer.WriteLine("Total income   : ${0:N2}", f.TotalIncome);
            writer.WriteLine();

            writer.WriteLine("Federal Tax:  ${0:N2}", tax);
            writer.WriteLine("Margin Rate:  {0:P2}", tax / f.TotalIncome);
            writer.WriteLine("Ordinary bracket:  {0:P2}", ordinalRate);
            writer.WriteLine("LongTerm bracket:  {0:P2}", longTermRate);
            writer.WriteLine("SocialSe taxable:  {0:P2}", socialSecurityTaxable);
        }
    }
}
