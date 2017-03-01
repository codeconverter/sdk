using CodeConverter.Common;
using CodeConverter.CSharp;
using CodeConverter.PowerShell;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CSharpToPowerShell.Test
{
    [TestFixture]
    public class LanguageTests
    {
        public IEnumerable<ISyntaxTreeVisitor> SyntaxTreeVisitors { get; private set; }
        public IEnumerable<CodeWriter> CodeWriters { get; private set; }
        public IEnumerable<string> TestCases { get; private set; }

        [SetUp]
        public void TestInit()
        {
            SyntaxTreeVisitors = new ISyntaxTreeVisitor[]
            {
                new PowerShellSyntaxTreeVisitor(),
                new CSharpSyntaxTreeVisitor()
            };

            CodeWriters = new CodeWriter[]
            {
                new PowerShellCodeWriter()
            };

            TestCases = new string[]
            {
                "AssignString",
                "AssignVariable",
                "MethodDeclaration",
                "MethodDeclarationWithArguments",
                "ObjectCreation",
                "ObjectCreationWithArguments"
            };
        }

        [Test]
        public void TestLanguages()
        {
            foreach(var syntaxTreeVisitor in SyntaxTreeVisitors)
            {
                foreach(var codeWriter in CodeWriters)
                {
                    if (syntaxTreeVisitor.Language == codeWriter.Language)
                    {
                        continue;
                    }

                    foreach (var testCase in TestCases) {
                        var source = ReadTestData(testCase, syntaxTreeVisitor.Language);
                        var target = ReadTestData(testCase, codeWriter.Language);

                        var ast = syntaxTreeVisitor.Visit(source);
                        var actual = codeWriter.Write(ast).Trim();

                        Assert.That(actual, Is.EqualTo(target));
                    }
                } 
            }
        }

        private string GetLanguageExtension(Language language)
        {
            switch (language)
            {
                case Language.CSharp:
                    return ".cs";
                case Language.PowerShell:
                    return ".ps1";
            }

            throw new NotImplementedException();
        }

        private string ReadTestData(string testName, Language language)
        {
            var languageName = Enum.GetName(typeof(Language), language);
            var extension = GetLanguageExtension(language);

            var testPath = $"Languages\\{languageName}\\{testName}{extension}";

            var testData = Path.Combine(TestContext.CurrentContext.WorkDirectory, testPath);

            if (!File.Exists(testData))
            {
                Assert.Fail($"No test data found at {testData}");
            }

            return File.ReadAllText(testData);
        }
    }
}
