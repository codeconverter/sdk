using CodeConverter.Common;
using CodeConverter.CSharp;
using CodeConverter.PowerShell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverter.Test
{
    public class CodeConverterFactory
    {
        private static IEnumerable<ISyntaxTreeVisitor> _parsers;
        private static IEnumerable<CodeWriter> _codeWriters;

        static CodeConverterFactory()
        {
            _parsers = new List<ISyntaxTreeVisitor>
            {
                new CSharpSyntaxTreeVisitor()
            };

            _codeWriters = new List<CodeWriter>
            {
                new PowerShellCodeWriter()
            };
        }

        public string Convert(string code, Language from, Language to)
        {
            var parser = _parsers.FirstOrDefault(m => m.Language == from);
            if (parser == null)
            {
                throw new NotImplementedException($"Parser for {Enum.GetName(typeof(Language), from)} is not implemented.");
            }

            var writer = _codeWriters.FirstOrDefault(m => m.Language == to);
            if (writer == null)
            {
                throw new NotImplementedException($"Code writer for {Enum.GetName(typeof(Language), to)} is not implemented.");
            }

            var ast = parser.Visit(code);
            return writer.Write(ast);
        }
    }
}
