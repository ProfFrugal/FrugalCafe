using System;
using System.Collections.Generic;

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
                OrdinaryDividens = 5_000,
                QualifiedDividens = 10_000,
                PretaxAccountWithdrawl= 30_000,
                SocialSecurity = 30_000
            };

            var tax = f.GetTax(2024);

            Console.WriteLine(tax);
        }
    }
}
