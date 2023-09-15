// Copyright (c) 2023 Feng Yuan for https://frugalcafe.beehiiv.com/
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace FrugalCafe
{
    public static class StringFormatterExtensions
    {
        [ThreadStatic]
        private static StringFormatter reusedFormatter;

        public static string OptimalFormat(this string format, object arg0)
        {
            return format.OptimalFormat(new ParamsArray<object>(arg0));
        }

        public static string OptimalFormat(this string format, object arg0, object arg1)
        {
            return format.OptimalFormat(new ParamsArray<object>(arg0, arg1));
        }

        public static string OptimalFormat(this string format, object arg0, object arg1, object arg2)
        {
            return format.OptimalFormat(new ParamsArray<object>(arg0, arg1, arg2));
        }

        public static string OptimalFormat(this string format, params object[] args)
        {
            return format.OptimalFormat(new ParamsArray<object>(args));
        }

        public static string OptimalFormat(this string format, ParamsArray<object> args)
        {
            StringFormatter formatter = reusedFormatter ?? new StringFormatter();

            reusedFormatter = null;

            formatter.AppendFormat(format, args);

            string result = formatter.ToString();

            formatter.Clear(true);

            reusedFormatter = formatter;

            return result;
        }

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

            while (true)
            {
                while (pos < len)
                {
                    int p = pos;

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

                    builder.Append(format, p);
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

                string argString = null;

                if (arg is IFormattable formattableArg)
                {
                    string sFmt = null;

                    if (fmtBuilder != null)
                    {
                        sFmt = fmtBuilder.ToString();
                    }

                    argString = formattableArg.ToString(sFmt, provider);
                }
                else if (arg != null)
                {
                    argString = arg.ToString();
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
