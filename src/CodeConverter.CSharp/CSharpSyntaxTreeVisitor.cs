using CodeConverter.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeConverter.CSharp
{
    public class CSharpSyntaxTreeVisitor : ISyntaxTreeVisitor
    {
        public Language Language => Language.CSharp;

        public Node Visit(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);

            var root = tree.GetCompilationUnitRoot();
            var visitor = new Visitor();
            root.Accept(visitor);

            return visitor.Node;
        }
    }

    internal class Visitor : CSharpSyntaxVisitor
    {
        private Node _currentNode;
        private static Dictionary<string, BinaryOperator> _operatorMap;

        static Visitor()
        {
            _operatorMap = new Dictionary<string, BinaryOperator>
            {
                { "==", BinaryOperator.Equal },
                { "!=", BinaryOperator.NotEqual },
                { ">", BinaryOperator.GreaterThan },
                { ">=", BinaryOperator.GreaterThanEqualTo },
                { "<", BinaryOperator.LessThan},
                { "<=", BinaryOperator.LessThanEqualTo},
                { "&&", BinaryOperator.And},
                { "||", BinaryOperator.Or},
                { "|", BinaryOperator.Bor},
                { "-", BinaryOperator.Minus },
                { "+", BinaryOperator.Plus },
                { "!", BinaryOperator.Not }
            };
        }

        public Node Node => _currentNode;

        public Node VisitSyntaxNode(CSharpSyntaxNode node)
        {
            Visit(node);
            return _currentNode;
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var left = VisitSyntaxNode(node.Left);
            var right = VisitSyntaxNode(node.Right);

            _currentNode = new Assignment(left, right);
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);

            _currentNode = new Argument(expression);
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            var arguments = new List<Node>();
            foreach (var argument in node.Arguments)
            {
                var argumentNode = VisitSyntaxNode(argument);

                arguments.Add(argumentNode);
            }

            _currentNode = new ArgumentList(arguments);
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            //Visit(node.Expression);
            //_builder.Append(".Result");
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var left = VisitSyntaxNode(node.Left);
            string op = node.OperatorToken.Text;

            var @operator = BinaryOperator.Unknown;

            if (_operatorMap.ContainsKey(op))
            {
                @operator = _operatorMap[op];
            }

            var right = VisitSyntaxNode(node.Right);

            _currentNode = new BinaryExpression(left, @operator, right);
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            var arguments = new List<Argument>();
            foreach(var argument in node.Arguments)
            {
                arguments.Add(VisitSyntaxNode(argument) as Argument);
            }

            _currentNode = new BracketedArgumentList(new ArgumentList(arguments));
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            _currentNode = new Break();
        }

        public override void VisitBlock(BlockSyntax node)
        {
            var statements = new List<Node>();
            foreach (var statement in node.Statements)
            {
                var statementNode = VisitSyntaxNode(statement);
                statements.Add(statementNode);
            }

            _currentNode = new Block(statements);
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            var expresion = VisitSyntaxNode(node.Expression);

            _currentNode = new Cast(node.Type.GetText().ToString().Trim(), expresion);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            Node declaration = null;
            if (node.Declaration != null)
            {
                declaration = VisitSyntaxNode(node.Declaration);
            }

            var block = VisitSyntaxNode(node.Block) as Block;
            _currentNode = new Catch(declaration, block);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            var type = node.Type.ToString();
            var identifier = node.Identifier.ValueText;

            _currentNode = new CatchDeclaration(type, identifier);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var name = node.Identifier.ToString();
            var members = new List<Node>();
            foreach(var member in node.Members)
            {
                members.Add(VisitSyntaxNode(member));
            }

            _currentNode = new ClassDeclaration(name, members);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var nodes = new List<Node>();
            foreach(var member in node.Members)
            {
                nodes.Add(VisitSyntaxNode(member));
            }

            _currentNode = new Block(nodes);
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            _currentNode = null;
            base.DefaultVisit(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);
            var argumentList = VisitSyntaxNode(node.ArgumentList) as ArgumentList;

            _currentNode = new ElementAccess(expression, argumentList);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            var statement = VisitSyntaxNode(node.Statement);
            _currentNode = new ElseClause(statement);
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            _currentNode = VisitSyntaxNode(node.Value);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            _currentNode = VisitSyntaxNode(node.Expression);
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            var body = VisitSyntaxNode(node.Block);

            _currentNode = new Finally(body);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            Node declaration = null;
            var initializers = new List<Node>();
            var incrementors = new List<Node>();
            Node condition = null;
            Node statement = null;

            if (node.Declaration != null)
            {
                declaration = VisitSyntaxNode(node.Declaration);
            }
            else
            {
                foreach (var initializer in node.Initializers)
                {
                    initializers.Add(VisitSyntaxNode(initializer));
                }
            }

            condition = VisitSyntaxNode(node.Condition);

            foreach (var incrementor in node.Incrementors)
            {
                incrementors.Add(VisitSyntaxNode(incrementor));
            }

            statement = VisitSyntaxNode(node.Statement);

            _currentNode = new ForStatement(declaration, initializers, incrementors, condition, statement);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);
            var statement = VisitSyntaxNode(node.Statement);
            var identifier = node.Identifier.Text;

            _currentNode = new ForEachStatement(identifier, expression, statement);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            _currentNode = new IdentifierName(node.Identifier.ToString());
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            var condition = VisitSyntaxNode(node.Condition);
            var statement = VisitSyntaxNode(node.Statement);
            ElseClause elseClause = null;

            if (node.Else != null)
            {
                elseClause = VisitSyntaxNode(node.Else) as ElseClause;
            }

            _currentNode = new IfStatement(condition, statement, elseClause);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);
            var argumentList = VisitSyntaxNode(node.ArgumentList) as ArgumentList;
            _currentNode = new Invocation(expression, argumentList);
        }
        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            _currentNode = new Literal(node.Token.ToString());
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            _currentNode = VisitSyntaxNode(node.Declaration);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);
            var identifier = node.Name.ToString();
            _currentNode = new MemberAccess(expression, identifier);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var name = node.Identifier.ToString();
            var parameters = new List<Parameter>();
            foreach (var parameter in node.ParameterList.Parameters)
            {
                parameters.Add(VisitSyntaxNode(parameter) as Parameter);
            }

            var body = VisitSyntaxNode(node.Body);

            _currentNode = new MethodDeclaration(name, parameters, body);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            var members = new List<Node>();
            foreach(var member in node.Members)
            {
                members.Add(VisitSyntaxNode(member));
            }

            _currentNode = new Namespace(node.Name.ToString(), members);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var type = node.Type.ToString();
            var argumentList = VisitSyntaxNode(node.ArgumentList) as ArgumentList;
            _currentNode = new ObjectCreation(type, argumentList);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            _currentNode = new Parameter(node.Type.ToString(), node.Identifier.ToString());
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);
            _currentNode = new ParenthesizedExpression(expression);
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            var operand = VisitSyntaxNode(node.Operand);
            var @operator = node.OperatorToken.Text;
            _currentNode = new PostfixUnaryExpression(operand, @operator);
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            var operand = VisitSyntaxNode(node.Operand);
            var @operator = node.OperatorToken.Text;
            _currentNode = new PrefixUnaryExpression(operand, @operator);
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            var block = VisitSyntaxNode(node.Block) as Block;
            var catches = new List<Catch>();
            foreach(var @catch in node.Catches)
            {
                catches.Add(VisitSyntaxNode(@catch) as Catch);
            }

            var fin = VisitSyntaxNode(node.Finally) as Finally;

            _currentNode = new Try(block, catches, fin);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            var expression = VisitSyntaxNode(node.Expression);
            _currentNode = new ReturnStatement(expression);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            var type = node.Type.ToString();
            if (node.Type.ToString() == "var")
            {
                type = null;
            }

            var variables = new List<VariableDeclarator>();
            foreach(var variable in node.Variables)
            {
                variables.Add(VisitSyntaxNode(variable) as VariableDeclarator);
            }

            _currentNode = new VariableDeclaration(type, variables);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var name = node.Identifier.ToString();
            var initializer = VisitSyntaxNode(node.Initializer);

            _currentNode = new VariableDeclarator(name, initializer);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            var condition = VisitSyntaxNode(node.Condition);
            var statement = VisitSyntaxNode(node.Statement);

            _currentNode = new While(condition, statement);
        }

    }
}
