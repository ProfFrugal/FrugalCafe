using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    public interface ISimpleStringBuilder
    {
        /// <summary>
        /// Append a string from arguments
        /// </summary>
        void Append(string value);

        /// <summary>
        /// Append a character from formatting string.
        /// </summary>
        void Append(string format, int pos);

        /// <summary>
        /// Add white space padding
        /// </summary>
        void Pad(int repeat);
    }
}
