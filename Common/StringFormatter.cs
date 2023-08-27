using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace FrugalCafe
{
    public static class StringFormatter
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void FormatError()
        {
            throw new FormatException("Format string is not in correct format.");
        }

        public static void AppendFormat<T>(
            this T builder, 
            string format, 
            ParamsArray<object> args, 
            IFormatProvider provider = null)
            where T : ISimpleStringBuilder
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            int pos = 0;
            int len = format.Length;
            char ch = '\x0';

            ICustomFormatter cf = null;

            if (provider != null)
            {
                cf = (ICustomFormatter)provider.GetFormat(typeof(ICustomFormatter));
            }

            while (true)
            {
                int p = pos;

                while (pos < len)
                {
                    ch = format[pos];
                    pos++;

                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}') // Treat as escape character for }}
                        {
                            pos++;
                        }
                        else
                        {
                            FormatError();
                        }
                    }
                    else if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{') // Treat as escape character for {{
                        {
                            pos++;
                        }
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    builder.Append(ch);
                }

                if (pos == len) break;

                pos++;

                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                {
                    FormatError();
                }

                int index = 0;

                do
                {
                    index = index * 10 + ch - '0';
                    pos++;

                    if (pos == len)
                    {
                        FormatError();
                    }

                    ch = format[pos];
                }
                while (ch >= '0' && ch <= '9' && index < 1000000);

                if (index >= args.Length)
                {
                    FormatError();
                }

                while (pos < len && (ch = format[pos]) == ' ') 
                { 
                    pos++; 
                }
                
                bool leftJustify = false;
                int width = 0;

                if (ch == ',')
                {
                    pos++;

                    while (pos < len && format[pos] == ' ')
                    { 
                        pos++; 
                    }

                    if (pos == len)
                    {
                        FormatError();
                    }

                    ch = format[pos];

                    if (ch == '-')
                    {
                        leftJustify = true;
                        pos++;

                        if (pos == len)
                        {
                            FormatError();
                        }

                        ch = format[pos];
                    }

                    if (ch < '0' || ch > '9')
                    {
                        FormatError();
                    }

                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;

                        if (pos == len)
                        {
                            FormatError();
                        }

                        ch = format[pos];
                    }
                    while (ch >= '0' && ch <= '9' && width < 1000000);
                }

                while (pos < len && (ch = format[pos]) == ' ')
                {
                    pos++;
                }

                object arg = args[index];
                StringBuilder fmtBuilder = null;

                if (ch == ':')
                {
                    pos++;
                    p = pos;

                    while (true)
                    {
                        if (pos == len)
                        {
                            FormatError();
                        }

                        ch = format[pos];
                        pos++;

                        if (ch == '{')
                        {
                            if (pos < len && format[pos] == '{')  // Treat as escape character for {{
                            {
                                pos++;
                            }
                            else
                            {
                                FormatError();
                            }
                        }
                        else if (ch == '}')
                        {
                            if (pos < len && format[pos] == '}')  // Treat as escape character for }}
                            {
                                pos++;
                            }
                            else
                            {
                                pos--;
                                break;
                            }
                        }

                        if (fmtBuilder == null)
                        {
                            fmtBuilder = StringBuilderExtensions.AcquireBuilder();
                        }

                        fmtBuilder.Append(ch);
                    }
                }

                if (ch != '}')
                {
                    FormatError();
                }

                pos++;

                string sFmt = null;
                string argString = null;

                if (cf != null)
                {
                    if (fmtBuilder != null)
                    {
                        sFmt = fmtBuilder.ToString();
                    }

                    argString = cf.Format(sFmt, arg, provider);
                }

                if (argString == null)
                {
                    IFormattable formattableArg = arg as IFormattable;

                    if (formattableArg != null)
                    {
                        if (sFmt == null && fmtBuilder != null)
                        {
                            sFmt = fmtBuilder.ToString();
                        }

                        argString = formattableArg.ToString(sFmt, provider);
                    }
                    else if (arg != null)
                    {
                        argString = arg.ToString();
                    }
                }

                if (fmtBuilder != null)
                {
                    fmtBuilder.Release();
                }

                if (argString == null)
                {
                    argString = string.Empty;
                }

                int pad = width - argString.Length;

                if (!leftJustify && pad > 0)
                {
                    builder.Pad(pad);
                }

                builder.Append(argString);

                if (leftJustify && pad > 0)
                {
                    builder.Pad(pad);
                }
            }
        }
    }
}
