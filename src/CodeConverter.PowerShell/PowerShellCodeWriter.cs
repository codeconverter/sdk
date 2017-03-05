using CodeConverter.Common;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverter.PowerShell
{
    public class PowerShellCodeWriter : CodeWriter
    {
        private static Dictionary<BinaryOperator, string> _operatorMap;

        static PowerShellCodeWriter()
        {
            _operatorMap = new Dictionary<BinaryOperator, string>
            {
                { BinaryOperator.Equal, " -eq " },
                { BinaryOperator.NotEqual, " -ne " },
                { BinaryOperator.GreaterThan, " -gt " },
                { BinaryOperator.LessThan, " -lt " },
                { BinaryOperator.LessThanEqualTo, " -le " },
                { BinaryOperator.GreaterThanEqualTo, " -ge " },
                { BinaryOperator.And, " -and " },
                { BinaryOperator.Or, " -or " },
                { BinaryOperator.Bor, " -bor " },
                { BinaryOperator.Minus, " - " },
                { BinaryOperator.Plus, " + " },
                { BinaryOperator.Not, " -not " }
            };
        }

        public override Language Language => Language.PowerShell;

        public override void VisitAssignment(Assignment node)
        {
            node.Left.Accept(this);
            Append("=");
            node.Right.Accept(this);
        }

        public override void VisitArgument(Argument node)
        {
            node.Expression.Accept(this);
        }

        public override void VisitArgumentList(ArgumentList node)
        {
            foreach (var argument in node.Arguments)
            {
                argument.Accept(this);

                Append(",");
            }

            //Remove trailing comma
            Builder.Remove(Builder.Length - 1, 1);
        }

        public override void VisitBinaryExpression(BinaryExpression node)
        {
            node.Left.Accept(this);

            if (_operatorMap.ContainsKey(node.Operator))
            {
                Append(_operatorMap[node.Operator]);
            }

            node.Right.Accept(this);
        }

        public override void VisitBlock(Block node)
        {
            foreach(var statement in node.Statements)
            {
                statement.Accept(this);
                NewLine();
            }
        }

        public override void VisitBracketedArgumentList(BracketedArgumentList node)
        {
            Append("[");

            foreach (var argument in node.Arguments)
            {
                argument.Accept(this);

                Append(",");
            }

            //Remove trailing comma
            Builder.Remove(Builder.Length - 1, 1);

            Append("]");
        }

        public override void VisitBreak(Break node)
        {
            Append("break");
        }

        public override void VisitCast(Cast node)
        {
            Append("[");
            Append(node.Type);
            Append("]");
            node.Expression.Accept(this);
        }

        public override void VisitCatch(Catch node)
        {
            Append("catch");
            if (node.Declaration != null)
            {
                Append(" ");
                node.Declaration.Accept(this);
            }
            NewLine();
            Append("{");
            Indent();
            NewLine();

            node.Block.Accept(this);

            Outdent();
            Append("}");
        }

        public override void VisitCatchDeclaration(CatchDeclaration node)
        {
            Append("[");
            Append(node.Type);
            Append("]");
        }

        public override void VisitElseClause(ElseClause node)
        {
            NewLine();
            Append("else");

            var isIf = node.Body is IfStatement;
            if (!isIf)
            {
                NewLine();
                Append("{");
                Indent();
                NewLine();
            }

            node.Body.Accept(this);

            if (!isIf)
            {
                Outdent();
                Append("}");
            }
        }

        public override void VisitFinally(Finally node)
        {
            Append("finally");
            NewLine();
            Append("{");
            Indent();
            NewLine();

            node.Body.Accept(this);

            Outdent();
            Append("}");
        }

        public override void VisitForStatement(ForStatement node)
        {
            Append("for(");

            if (node.Declaration != null)
            {
                node.Declaration.Accept(this);
            }
            else
            {
                foreach (var initializer in node.Initializers)
                {
                    initializer.Accept(this);
                }
            }

            Append(";");

            node.Condition.Accept(this);

            Append(";");

            foreach (var incrementor in node.Incrementors)
            {
                incrementor.Accept(this);
            }

            Append("){");

            node.Statement.Accept(this);

            Append("}");
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            Append("foreach($");
            Append(node.Identifier);
            Append(" in ");
            node.Expression.Accept(this);
            Append("){");
            node.Statement.Accept(this);
            Append("}");
        }

        public override void VisitIdentifierName(IdentifierName node)
        {
            Append("$");
            Append(node.Name);
        }

        public override void VisitIfStatement(IfStatement node)
        {
            Append("if (");
            node.Condition.Accept(this);
            Append(")");
            NewLine();
            Append("{");
            Indent();
            NewLine();

            node.Body.Accept(this);

            Outdent();
            Append("}");

            node.ElseClause?.Accept(this);
        }

        public override void VisitInvocation(Invocation node)
        {
            node.Expression.Accept(this);

            if (!node.Arguments.Arguments.Any())
            {
                Append("()");
            } else
            {
                Append("(");
                node.Arguments.Accept(this);
                Append(")");
            }
        }

        public override void VisitLiteral(Literal node)
        {
            if (node.Token == "true" || node.Token == "false")
                Append("$");

            Append(node.Token);
        }

        public override void VisitMemberAccess(MemberAccess node)
        {
            node.Expression.Accept(this);
            Append(".");
            Append(node.Identifier);
        }

        public override void VisitMethodDeclaration(MethodDeclaration node)
        {
            Append("function ");
            Append(node.Name);
            NewLine();
            Append("{");
            Indent();
            NewLine();

            if (node.Parameters.Any())
            {
                Append("param(");
                foreach (var parameter in node.Parameters)
                {
                    parameter.Accept(this);
                    Append(", ");
                }
                Builder.Remove(Builder.Length - 2, 2);
                Append(")");
                NewLine();
            }

            node.Body.Accept(this);

            Outdent();
            Append("}");
        }

        public override void VisitObjectCreation(ObjectCreation node)
        {
            var typeName = node.Type;

            Append("(New-Object -TypeName ");
            Append(typeName);

            if (!node.Arguments.Arguments.Any())
            {
                Append(")");
                return;
            };

            Append(" -ArgumentList ");

            VisitArgumentList(node.Arguments);

            Append(")");
        }

        public override void VisitParameter(Parameter node)
        {
            if (!string.IsNullOrEmpty(node.Type))
            {
                Append("[");
                Append(node.Type);
                Append("]");
            }

            Append("$");
            Append(node.Name);
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpression node)
        {
            Append("(");
            node.Expression.Accept(this);
            Append(")");
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpression node)
        {
            node.Operand.Accept(this);
            Append("++");
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpression node)
        {
            Append("++");
            node.Operand.Accept(this);
        }

        public override void VisitStringConstant(StringConstant node)
        {
            Append("\'" + node.Value + "\'");
        }

        public override void VisitTemplateStringConstant(TemplateStringConstant node)
        {
            Append("\"" + node.Value + "\"");
        }

        public override void VisitTry(Try node)
        {
            Append("try");
            NewLine();
            Append("{");
            Indent();
            NewLine();

            node.Block.Accept(this);

            Outdent();
            Append("}");
            foreach(var @catch in node.Catches)
            {
                NewLine();
                @catch.Accept(this);
            }

            if (node.Finally != null)
            {
                NewLine();
                node.Finally.Accept(this);
            }
            
        }

        public override void VisitReturnStatement(ReturnStatement node)
        {
            Append("return");
        }

        public override void VisitVariableDeclaration(VariableDeclaration node)
        {
            if (!string.IsNullOrEmpty(node.Type))
            {
                Append("[");
                Append(node.Type);
                Append("]");
            }
            
            foreach(var variable in node.Variables)
            {
                VisitVariableDeclarator(variable);
            }
        }

        public override void VisitVariableDeclarator(VariableDeclarator node)
        {
            Append("$");
            Append(node.Name);
            if (node.Initializer != null)
            {
                Append(" = ");
                node.Initializer.Accept(this);
            }
        }

        public override void VisitWhile(While node)
        {
            Append("while(");
            node.Condition.Accept(this);
            Append("){");
            node.Statement.Accept(this);
            Append("}");
        }
    }
}
