using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    public interface ISimpleStringBuilder
    {
        void Append(string value);
        void Append(char value, int repeat = 1);
    }
}
