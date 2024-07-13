using System;

namespace IncomeTax
{
    public enum TaxFilerClass
    {
        Single,
        MarriedFillingJointly,
        MarriedFillingSeparately,
        HeadOfHouseHold
    }

    public class TaxBracket
    {
        public TaxFilerClass Class { get; set; }
        public double Min { get; }
        public double Max { get; }
        public double Rate { get; }

        public TaxBracket(double from, double to, double rate)
        {
            Min = from;
            Max = to;
            Rate = rate;
        }

        public double GetTax(double baseIncome, ref double income)
        {
            if (baseIncome + income > Min)
            {
                var inbracket = Math.Min(baseIncome + income, Max) - Min;

                if (inbracket > income)
                {
                    inbracket = income;
                }

                income -= inbracket;

                return inbracket * Rate;
            }

            return 0;
        }
    }
}
