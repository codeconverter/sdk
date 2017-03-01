using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeConverter.Common;
using System.Linq;

namespace CodeConverter.CSharp.Test
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void NamespaceTest()
        {
            var parser = new CSharpSyntaxTreeVisitor();
            var unit = parser.Visit("namespace MyNamespace { }");

            Assert.IsInstanceOfType(unit, typeof(Block));
            var block = unit as Block;
            var ns = block.Statements.Cast<Namespace>().First();
            Assert.AreEqual("MyNamespace", ns.Name);
        }

        [TestMethod]
        public void ClassTest()
        {
            var parser = new CSharpSyntaxTreeVisitor();
            var unit = parser.Visit("namespace MyNamespace { class myClass { } }");

            var block = unit as Block;
            var classDecl = block.Statements.Cast<Namespace>().First().Members.Cast<ClassDeclaration>().First();
            Assert.AreEqual("myClass", classDecl.Name);
        }

        [TestMethod]
        public void MethodTest()
        {
            var parser = new CSharpSyntaxTreeVisitor();
            var unit = parser.Visit("namespace MyNamespace { " +
                "class myClass { " +
                " void MyMethod() {} " +
                "} }");

            var block = unit as Block;
            var method = block.Statements.Cast<Namespace>().First().Members.Cast<ClassDeclaration>().First().Members.Cast<MethodDeclaration>().First();
            Assert.AreEqual("MyMethod", method.Name);
        }

        [TestMethod]
        public void MethodWithParametersTest()
        {
            var block = Parse(@"namespace MyNamespace {
                class MyClass {
                    void MyMethod(string p1) {
                    
                    }
                }
            }");
            
            var method = block.Statements.Cast<Namespace>().First().Members.Cast<ClassDeclaration>().First().Members.Cast<MethodDeclaration>().First();
            Assert.AreEqual("MyMethod", method.Name);
            var parameter = method.Parameters.First();
            Assert.AreEqual("string", parameter.Type);
            Assert.AreEqual("p1", parameter.Name);

        }

        //[TestMethod]
        //public void VariableDeclarationTest()
        //{
        //    var block = Parse(@"namespace MyNamespace {
        //        class MyClass {
        //            void MyMethod(string p1) {
        //                int item;
        //            }
        //        }
        //    }");

        //    var method = block.Statements.Cast<Namespace>().First().Members.Cast<ClassDeclaration>().First().Members.Cast<MethodDeclaration>().First();
        //    var variableDecl = method.Body.Statements.Cast<VariableDeclaration>().First();
        //    Assert.AreEqual("int", variableDecl.Type);
        //    Assert.AreEqual("item", variableDecl.Variables.First().Name);
        //}

        //[TestMethod]
        //public void VariableDeclarationWithInitializerTest()
        //{
        //    var block = Parse(@"namespace MyNamespace {
        //        class MyClass {
        //            void MyMethod(string p1) {
        //                int item = 1;
        //            }
        //        }
        //    }");

        //    var method = block.Statements.Cast<Namespace>().First().Members.Cast<ClassDeclaration>().First().Members.Cast<MethodDeclaration>().First();
        //    var variableDecl = method.Body.Statements.Cast<VariableDeclaration>().First();
        //    var literal = variableDecl.Variables.First().Initializer as Literal;
        //    Assert.AreEqual("1", literal.Token);
        //}

        private Block Parse(string text)
        {
            var parser = new CSharpSyntaxTreeVisitor();
            var unit = parser.Visit(text);
            return unit as Block;
        }
    }
}
