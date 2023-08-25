using System;
using System.IO;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;

namespace FrugalCafe.Posts
{
    internal class TestICSharpDecompiler
    {
        private ICSharpCode.Decompiler.DecompilerSettings _settings = new ICSharpCode.Decompiler.DecompilerSettings();

        public void Test(Type type)
        {
            CSharpDecompiler decompiler = new CSharpDecompiler(type.Assembly.Location, _settings);

            string result = null;

            int count = 100;

            Console.WriteLine(type);

            PerfTest.MeasurePerf(
                () =>
                {
                    SyntaxTree syntaxTree = decompiler.DecompileType(
                        new ICSharpCode.Decompiler.TypeSystem.FullTypeName(type.FullName));

                    StringWriter w = new StringWriter();
                    syntaxTree.AcceptVisitor(
                        new CSharpOutputVisitor(w, _settings.CSharpFormattingOptions));

                    string decompilation_log = "decompilation_log";
                    object logger = "logger\r\n";

                    w.WriteLine("#if false // " + decompilation_log);
                    w.Write(logger.ToString());
                    w.WriteLine("#endif");

                    result = w.ToString();
                },
                "Decompile",
                count);

            Console.Write("{0} chars", result.Length);
        }
    }
}
