using System;
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

        public double GetRate(TaxFilerClass klass, double income)
        {
            var list = TaxBrackets[(int)klass];

            if (list == null)
            {
                throw new ArgumentOutOfRangeException(nameof(klass));
            }

            foreach (var bracket in list)
            {
                if ((income > bracket.Min) && (income <= bracket.Max))
                {
                    return bracket.Rate;
                }
            }

            return 0;
        }

        public double GetDeduction(int year, TaxFiler filer)
        {
            double deduction = StandardDeductions[(int)filer.FilerClass];

            if (deduction > 0)
            {
                AddSeniorDeduction(ref deduction, filer.Birthday1, year);
                AddSeniorDeduction(ref deduction, filer.Birthday2, year);
            }

            return deduction;
        }

        public double GetTax(double baseIncome, double taxableIncome, TaxFiler filer, int year, out double marginRate)
        {
            var brackets = TaxBrackets[(int)filer.FilerClass];

            if (brackets == null)
            {
                throw new ArgumentOutOfRangeException(nameof(filer));
            }

            double tax = 0;

            marginRate = 0;

            if (taxableIncome > 0)
            {
                for (int i = brackets.Length - 1; i >= 0; i--)
                {
                    var bracket = brackets[i];

                    double t = bracket.GetTax(baseIncome, ref taxableIncome);

                    if (t > 0)
                    {
                        if (marginRate == 0)
                        {
                            marginRate = bracket.Rate;
                        }

                        tax += t;

                        if (taxableIncome <= 0)
                        {
                            break;
                        }
                    }
                }
            }

            return tax;
        }

        private void AddSeniorDeduction(ref double deduction, DateTime? birthday, int year)
        {
            if (birthday.HasValue && (birthday.Value.Year <= (year - 65)))
            {
                deduction += 1950;
            }
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
                    new TaxBracket(0, 23_200, 0.10),
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
