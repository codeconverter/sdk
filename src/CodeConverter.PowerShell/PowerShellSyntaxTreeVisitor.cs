using CodeConverter.Common;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;

namespace CodeConverter.PowerShell
{
    public class PowerShellSyntaxTreeVisitor : ISyntaxTreeVisitor
    {
        public Language Language => Language.PowerShell;

        public Node Visit(string code)
        {
            var ast = Parser.ParseInput(code, out Token[] tokens, out ParseError[] errors);
            var visitor = new PowerShellAstVisitor();
            ast.Visit(visitor);
            return visitor.Node;
        }
    }

    internal class PowerShellAstVisitor : AstVisitor2
    {
        private static Dictionary<TokenKind, BinaryOperator> _operatorMap;

        static PowerShellAstVisitor()
        {
            _operatorMap = new Dictionary<TokenKind, BinaryOperator>
            {
                { TokenKind.And, BinaryOperator.And },
                { TokenKind.Equals, BinaryOperator.Equal }
            };
        }

        private Node _currentNode;
        public Node Node => _currentNode;

        public Node VisitSyntaxNode(Ast node)
        {
            node.Visit(this);
            return _currentNode;
        }

        public override AstVisitAction VisitAssignmentStatement(AssignmentStatementAst assignmentStatementAst)
        {
            var left = VisitSyntaxNode(assignmentStatementAst.Left);
            var right = VisitSyntaxNode(assignmentStatementAst.Right);
            _currentNode = new Assignment(left, right);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitBinaryExpression(BinaryExpressionAst binaryExpressionAst)
        {
            var left = VisitSyntaxNode(binaryExpressionAst.Left);
            var @operator = BinaryOperator.Unknown;

            if (_operatorMap.ContainsKey(binaryExpressionAst.Operator))
            {
                @operator = _operatorMap[binaryExpressionAst.Operator];
            }

            var right = VisitSyntaxNode(binaryExpressionAst.Right);

            _currentNode = new BinaryExpression(left, @operator, right);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitBlockStatement(BlockStatementAst blockStatementAst)
        {
            _currentNode = VisitSyntaxNode(blockStatementAst.Body);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitBreakStatement(BreakStatementAst breakStatementAst)
        {
            _currentNode = new Break();

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitCatchClause(CatchClauseAst catchClauseAst)
        {
            var body = VisitSyntaxNode(catchClauseAst.Body);
            var type = catchClauseAst.CatchTypes.FirstOrDefault();
            CatchDeclaration declaration = null;
            if (type != null)
            {
                var myType = VisitSyntaxNode(type);
                declaration = new CatchDeclaration(myType.ToString(), null);
            }

            _currentNode = new Catch(declaration, body as Block);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitConstantExpression(ConstantExpressionAst constantExpressionAst)
        {
            _currentNode = new Literal(constantExpressionAst.Value.ToString());
            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitContinueStatement(ContinueStatementAst continueStatementAst)
        {
            _currentNode = new Continue();
            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitForEachStatement(ForEachStatementAst forEachStatementAst)
        {
            var body = VisitSyntaxNode(forEachStatementAst.Body);
            var condition = VisitSyntaxNode(forEachStatementAst.Condition);
            var variable = VisitSyntaxNode(forEachStatementAst.Variable);

            //TODO: 

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst functionDefinitionAst)
        {
            var body = VisitSyntaxNode(functionDefinitionAst.Body);
            var name = functionDefinitionAst.Name;
            var parameters = new List<Parameter>();
            foreach(var parameter in functionDefinitionAst.Parameters)
            {
                parameters.Add(new Parameter(parameter.StaticType.ToString(), parameter.Name.ToString()));
            }

            _currentNode = new MethodDeclaration(name, parameters, body);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitIfStatement(IfStatementAst ifStmtAst)
        {
            var firstCondition = ifStmtAst.Clauses.First();
            var condition_maybe = VisitSyntaxNode(firstCondition.Item1);
            var body = VisitSyntaxNode(firstCondition.Item2);

            // If
            var ifStatement = new IfStatement(condition_maybe, body);

            var previousIf = ifStatement;

            // Else ifs
            foreach (var clause in ifStmtAst.Clauses)
            {
                condition_maybe = VisitSyntaxNode(firstCondition.Item1);
                body = VisitSyntaxNode(firstCondition.Item2);
                var nextCondition = new IfStatement(condition_maybe, body);
                previousIf.ElseClause = new ElseClause(nextCondition);
                previousIf = nextCondition;
            }

            // Else
            if (ifStmtAst.ElseClause != null)
            {
                var statements = new List<Node>();
                foreach(var statement in ifStmtAst.ElseClause.Statements)
                {
                    var statementNode = VisitSyntaxNode(statement);
                    statements.Add(statementNode);
                }
                body = new Block(statements);
                previousIf.ElseClause = new ElseClause(body);
            }

            _currentNode = ifStatement;

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitInvokeMemberExpression(InvokeMemberExpressionAst methodCallAst)
        {
            var arguments = new List<Argument>();
            foreach(var argument in methodCallAst.Arguments)
            {
                var arg = VisitSyntaxNode(argument);
                var ar = new Argument(arg);
                arguments.Add(ar);
            }

            var expression = VisitSyntaxNode(methodCallAst.Expression);

            var methodName = methodCallAst.Member.ToString();

            var argumentList = new ArgumentList(arguments);

            _currentNode = new Invocation(expression, argumentList);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitParameter(ParameterAst parameterAst)
        {
            var name = parameterAst.Name.ToString();
            var type = parameterAst.StaticType?.Name;
            _currentNode = new Parameter(type, name);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitStringConstantExpression(StringConstantExpressionAst stringConstantExpressionAst)
        {
            _currentNode = new StringConstant(stringConstantExpressionAst.Value);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitExpandableStringExpression(ExpandableStringExpressionAst expandableStringExpressionAst)
        {
            _currentNode = new TemplateStringConstant(expandableStringExpressionAst.Value);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitReturnStatement(ReturnStatementAst returnStatementAst)
        {
            _currentNode = new ReturnStatement();
            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitStatementBlock(StatementBlockAst statementBlockAst)
        {
            var statements = new List<Node>();
            foreach(var statement in statementBlockAst.Statements)
            {
                statements.Add(VisitSyntaxNode(statement));
            }

            _currentNode = new Block(statements);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitTryStatement(TryStatementAst tryStatementAst)
        {
            var tryBody = VisitSyntaxNode(tryStatementAst.Body) as Block;

            var catches = new List<Catch>();
            foreach(var catchClause in  tryStatementAst.CatchClauses)
            {
                var catchNode = VisitSyntaxNode(catchClause) as Catch;
                catches.Add(catchNode);
            }

            var fin = VisitSyntaxNode(tryStatementAst.Finally) as Finally;

            _currentNode = new Try(tryBody, catches, fin);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitUnaryExpression(UnaryExpressionAst unaryExpressionAst)
        {
            var child = VisitSyntaxNode(unaryExpressionAst.Child);
            if (unaryExpressionAst.TokenKind == TokenKind.PostfixPlusPlus)
            {
                _currentNode = new PostfixUnaryExpression(child, "++");
            }
            else if (unaryExpressionAst.TokenKind == TokenKind.PlusPlus)
            {
                _currentNode = new PrefixUnaryExpression(child, "++");
            }
            else if (unaryExpressionAst.TokenKind == TokenKind.PostfixMinusMinus)
            {
                _currentNode = new PostfixUnaryExpression(child, "--");
            }
            else if (unaryExpressionAst.TokenKind == TokenKind.MinusMinus)
            {
                _currentNode = new PrefixUnaryExpression(child, "--");
            }

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitWhileStatement(WhileStatementAst whileStatementAst)
        {
            var body = VisitSyntaxNode(whileStatementAst.Body);
            var condition = VisitSyntaxNode(whileStatementAst.Condition);

            _currentNode = new While(condition, body);

            return AstVisitAction.SkipChildren;
        }

        public override AstVisitAction VisitParenExpression(ParenExpressionAst parenExpressionAst)
        {
            var expression = VisitSyntaxNode(parenExpressionAst.Pipeline);
            _currentNode = new ParenthesizedExpression(expression);
            return AstVisitAction.SkipChildren;
        }
    }

}
