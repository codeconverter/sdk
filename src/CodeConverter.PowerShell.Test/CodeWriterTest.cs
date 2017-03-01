using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeConverter.Common;
using System.Collections.Generic;

namespace CodeConverter.PowerShell.Test
{
    [TestClass]
    public class CodeWriterTest
    {
        [TestMethod]
        public void ShouldWriteMethod()
        {
            var method = new MethodDeclaration("Hey", new Parameter[0], new Block());
            var cls = new ClassDeclaration("MyClass", new Node[] { method });
            var ns = new Namespace("test", new List<Node> { cls });

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {}", output);
        }

        [TestMethod]
        public void ShouldWriteMethodWithParameters()
        {
            var parameter = new Parameter("MyType", "MyParameterName");
            var parameter2 = new Parameter("MyType2", "MyParameterName2");
            var method = new MethodDeclaration("Hey", new Parameter[] { parameter, parameter2 }, new Block());
            var cls = new ClassDeclaration("MyClass", new Node[] { method });
            var ns = new Namespace("test", new List<Node> { cls });

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {param([MyType]$MyParameterName,[MyType2]$MyParameterName2)}", output);
        }

        [TestMethod]
        public void ShouldCreateObject()
        {
            var objectCreation = new ObjectCreation("String", new ArgumentList(new Node[0]));
            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(new[] { objectCreation}));
            var cls = new ClassDeclaration("MyClass", new Node[] { method });
            var ns = new Namespace("test", new List<Node> { cls });

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {(New-Object -TypeName String)}", output);
        }
        
        [TestMethod]
        public void ShouldCreateObjectWithArguments()
        {
            var argument = new Argument(new Literal("1"));
            var objectCreation = new ObjectCreation("String", new ArgumentList(new[] { argument }));
            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(new[] { objectCreation }));
            var cls = new ClassDeclaration("MyClass", new Node[] { method });
            var ns = new Namespace("test", new List<Node> { cls });

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {(New-Object -TypeName String -ArgumentList 1)}", output);
        }

        [TestMethod]
        public void ShouldCreateObjectMultipleWithArguments()
        {
            var argument = new Argument(new Literal("1"));
            var argument2 = new Argument(new Literal("2"));
            var objectCreation = new ObjectCreation("String", new ArgumentList(new[] { argument, argument2 }));
            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(new[] { objectCreation }));
            var cls = new ClassDeclaration("MyClass", new Node[] { method });
            var ns = new Namespace("test", new List<Node> { cls });

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {(New-Object -TypeName String -ArgumentList 1,2)}", output);
        }

        [TestMethod]
        public void ShouldAssignVariable()
        {
            var variableDeclaration = new VariableDeclaration("String", new VariableDeclarator("myVar", new Literal("\"1\"")));

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(variableDeclaration));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls );

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {[String]$myVar = \"1\"}", output);
        }

        [TestMethod]
        public void ShouldExecuteMethod()
        {
            var identifierName = new IdentifierName("myVariable");
            var memberAccess = new MemberAccess(identifierName, "MyMethod");
            var argumentList = new ArgumentList();
            var invocation = new Invocation(memberAccess, argumentList);
            
            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(invocation));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {$myVariable.MyMethod()}", output);
        }

        [TestMethod]
        public void ShouldExecuteMethodWithArguments()
        {
            var identifierName = new IdentifierName("myVariable");
            var memberAccess = new MemberAccess(identifierName, "MyMethod");
            var argumentList = new ArgumentList(new Argument(new Literal("1")));
            var invocation = new Invocation(memberAccess, argumentList);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(invocation));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {$myVariable.MyMethod(1)}", output);
        }

        [TestMethod]
        public void ShouldExecuteMethodWithMultipleArguments()
        {
            var identifierName = new IdentifierName("myVariable");
            var memberAccess = new MemberAccess(identifierName, "MyMethod");
            var argumentList = new ArgumentList(new Argument(new Literal("1")), new Argument(new Literal("2")));
            var invocation = new Invocation(memberAccess, argumentList);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(invocation));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {$myVariable.MyMethod(1,2)}", output);
        }

        [TestMethod]
        public void ShouldExecuteMethodWithVariableArguments()
        {
            var identifierName = new IdentifierName("myVariable");
            var memberAccess = new MemberAccess(identifierName, "MyMethod");
            var argumentList = new ArgumentList(new Argument(new IdentifierName("myOtherVariable")));
            var invocation = new Invocation(memberAccess, argumentList);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(invocation));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {$myVariable.MyMethod($myOtherVariable)}", output);
        }

        [TestMethod]
        public void ShouldAccessProperty ()
        {
            var identifierName = new IdentifierName("myVariable");
            var memberAccess = new MemberAccess(identifierName, "MyProperty");
            var identifierName2 = new IdentifierName("myOtherVariable");
            var assignment = new Assignment(identifierName2, memberAccess);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(assignment));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {$myOtherVariable=$myVariable.MyProperty}", output);
        }

        [TestMethod]
        public void ShouldSetProperty()
        {
            var identifierName = new IdentifierName("myVariable");
            var memberAccess = new MemberAccess(identifierName, "MyProperty");
            var identifierName2 = new IdentifierName("myOtherVariable");
            var assignment = new Assignment(memberAccess, identifierName2);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(assignment));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {$myVariable.MyProperty=$myOtherVariable}", output);
        }

        [TestMethod]
        public void ShouldCast()
        {
            var cast = new Cast("int", new Literal("\"1\""));
            var variableDeclarator = new VariableDeclarator("myVariable", cast);
            var variableDeclaration = new VariableDeclaration("int", variableDeclarator);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(variableDeclaration));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {[int]$myVariable = [int]\"1\"}", output);
        }

        [TestMethod]
        public void ShouldWriteIf()
        {
            var condition = new BinaryExpression(new Literal("1"), BinaryOperator.Equal, new Literal("1"));
            var body = new VariableDeclaration("String", new VariableDeclarator("myVar", new Literal("\"1\"")));
            var ifStatement = new IfStatement(condition, body);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(ifStatement));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {if(1 -eq 1){[String]$myVar = \"1\"}}", output);
        }

        [TestMethod]
        public void ShouldWriteElse()
        {
            var condition = new BinaryExpression(new Literal("1"), BinaryOperator.Equal, new Literal("1"));
            var body = new VariableDeclaration("String", new VariableDeclarator("myVar", new Literal("\"1\"")));

            var elseClause = new ElseClause(body);
            var ifStatement = new IfStatement(condition, body, elseClause);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(ifStatement));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {if(1 -eq 1){[String]$myVar = \"1\"}else{[String]$myVar = \"1\"}}", output);
        }

        [TestMethod]
        public void ShouldWriteElseIf()
        {
            var condition = new BinaryExpression(new Literal("1"), BinaryOperator.Equal, new Literal("1"));
            var body = new VariableDeclaration("String", new VariableDeclarator("myVar", new Literal("\"1\"")));
            var elseIfStatement = new IfStatement(condition, body);
            var elseClause = new ElseClause(elseIfStatement);
            var ifStatement = new IfStatement(condition, body, elseClause);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(ifStatement));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual("function Hey {if(1 -eq 1){[String]$myVar = \"1\"}elseif(1 -eq 1){[String]$myVar = \"1\"}}", output);
        }

        [TestMethod]
        public void ShouldWriteEq()
        {
            BinaryOperatorTest("-eq", BinaryOperator.Equal);
        }

        [TestMethod]
        public void ShouldWriteGt()
        {
            BinaryOperatorTest("-gt", BinaryOperator.GreaterThan);
        }

        [TestMethod]
        public void ShouldWriteLt()
        {
            BinaryOperatorTest("-lt", BinaryOperator.LessThan);
        }

        [TestMethod]
        public void ShouldWriteGe()
        {
            BinaryOperatorTest("-ge", BinaryOperator.GreaterThanEqualTo);
        }

        [TestMethod]
        public void ShouldWriteLe()
        {
            BinaryOperatorTest("-le", BinaryOperator.LessThanEqualTo);
        }

        private void BinaryOperatorTest(string expectedCondition, BinaryOperator @opeartor)
        {
            var condition = new BinaryExpression(new Literal("1"), @opeartor, new Literal("1"));
            var body = new VariableDeclaration("String", new VariableDeclarator("myVar", new Literal("\"1\"")));
            var ifStatement = new IfStatement(condition, body);

            var method = new MethodDeclaration("Hey", new Parameter[0], new Block(ifStatement));
            var cls = new ClassDeclaration("MyClass", method);
            var ns = new Namespace("test", cls);

            var writer = new PowerShellCodeWriter();
            var output = writer.Write(ns);
            Assert.AreEqual($"function Hey {{if(1 {expectedCondition} 1){{[String]$myVar = \"1\"}}}}", output);
        }
    }
}
