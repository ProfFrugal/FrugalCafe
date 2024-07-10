
using System;

namespace IncomeTax
{
    internal class SocailSecurityTaxRange
    {
        public double Start50;
        public double Start85;

        public double GetTaxable(double modifiedAGI, double benefitIncome)
        {
            if (modifiedAGI < Start50)
            {
                return 0;
            }

            double method1 = (modifiedAGI - Start50) * 0.50;

            if (modifiedAGI > Start85)
            {
                method1 += (modifiedAGI - Start85) * 0.35;
            }

            double method2 = benefitIncome * 0.85;

            return Math.Min(method1, method2);
        }
    }
}
