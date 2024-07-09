﻿using System;
using System.Collections.Generic;

namespace IncomeTax
{
    public class TaxSchedule
    {
        private TaxBracket[][] TaxBrackets;
        private double[] StandardDeductions;


        public TaxSchedule()
        {
            TaxBrackets = new TaxBracket[4][];
            StandardDeductions = new double[4];
        }

        public void AddTaxBrackets(TaxFilerClass klass, double standardDeduction, TaxBracket[] taxBrackets)
        {
            TaxBrackets[(int)klass] = taxBrackets;
            StandardDeductions[(int)klass] = standardDeduction;

            foreach (var bracket in taxBrackets)
            {
                bracket.Class = klass;
            }
        }

        public double GetTax(TaxFilerClass klass, double taxableIncome)
        {
            var list = TaxBrackets[(int)klass];

            if (list == null)
            {
                throw new ArgumentOutOfRangeException(nameof(klass));
            }

            taxableIncome -= StandardDeductions[(int)klass];

            double tax = 0;

            if (taxableIncome > 0)
            {
                foreach (var bracket in list)
                {
                    tax += bracket.GetTax(taxableIncome);
                }
            }

            return tax;
        }
    }

    public class TaxYear
    {
        public static Dictionary<int, TaxYear> TaxYears;

        public TaxSchedule OrdinalIncome { get; }
        public TaxSchedule LongTermCapitcalGain { get; }

        public int Year { get; }

        public TaxYear(int year)
        {
            OrdinalIncome = new TaxSchedule();
            LongTermCapitcalGain = new TaxSchedule();
            Year = year;
        }

        static TaxYear()
        {
            TaxYears = new Dictionary<int, TaxYear>();

            var y2024 = new TaxYear(2024);

            y2024.OrdinalIncome.AddTaxBrackets(TaxFilerClass.MarriedFillingJointly, 29_200, new TaxBracket[]
                {
                    new TaxBracket(0, 23_200, 0.1),
                    new TaxBracket(23_200, 94_300, 0.12),
                    new TaxBracket(94_300, 201_050, 0.22),
                    new TaxBracket(201_050, 383_900, 0.24),
                    new TaxBracket(383_900, 487_450, 0.32),
                    new TaxBracket(487_450, 731_200, 0.35),
                    new TaxBracket(731_200, double.MaxValue, 0.37),
                }
            );

            y2024.LongTermCapitcalGain.AddTaxBrackets(TaxFilerClass.MarriedFillingJointly, 0, new TaxBracket[]
                {
                    new TaxBracket(94_050, 583_750, 0.15),
                    new TaxBracket(583_750, double.MaxValue, 0.20),
                }
            );

            TaxYears.Add(2024, y2024);
        }
    }
}