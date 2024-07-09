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

        public double GetTax(double income)
        {
            if ((income > Min) && (income <= Max))
            {
                income = Math.Min(income, Max) - Min;

                return income * Rate;
            }

            return 0;
        }
    }
}
