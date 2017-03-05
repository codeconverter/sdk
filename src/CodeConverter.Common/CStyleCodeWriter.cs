using System.Collections.Generic;
using System.Linq;

namespace CodeConverter.Common
{
    public abstract class CStyleCodeWriter : CodeWriter
    {
        protected abstract Dictionary<BinaryOperator, string> OperatorMap { get; }
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

            if (OperatorMap.ContainsKey(node.Operator))
            {
                Append(OperatorMap[node.Operator]);
            }

            node.Right.Accept(this);
        }

        public override void VisitBlock(Block node)
        {
            foreach (var statement in node.Statements)
            {
                statement.Accept(this);
                NewLine();
            }
        }

        public override void VisitBreak(Break node)
        {
            Append("break");
        }

        public override void VisitCast(Cast node)
        {
            Append("(");
            Append(node.Type);
            Append(")");
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
            Append("(");
            Append(node.Type);
            Append(")");
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

            Append("; ");

            node.Condition.Accept(this);

            Append("; ");

            foreach (var incrementor in node.Incrementors)
            {
                incrementor.Accept(this);
            }

            Append(")");
            NewLine();
            Append("{");
            Indent();
            NewLine();

            node.Statement.Accept(this);

            Outdent();
            Append("}");
        }

        public override void VisitForEachStatement(ForEachStatement node)
        {
            Append("foreach(");
            node.Identifier.Accept(this);
            Append(" in ");
            node.Expression.Accept(this);
            Append(")");
            NewLine();
            Append("{");
            Indent();
            NewLine();
            node.Statement.Accept(this);
            Outdent();
            Append("}");
        }

        public override void VisitIdentifierName(IdentifierName node)
        {
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
            }
            else
            {
                Append("(");
                node.Arguments.Accept(this);
                Append(")");
            }
        }

        public override void VisitLiteral(Literal node)
        {
            Append(node.Token);
        }

        public override void VisitMemberAccess(MemberAccess node)
        {
            node.Expression.Accept(this);
            Append(".");
            Append(node.Identifier);
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
            Append("\"" + node.Value + "\"");
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
            foreach (var @catch in node.Catches)
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

            if (node.Expression != null)
            {
                Append(" ");
                node.Expression.Accept(this);
            }

        }

        public override void VisitWhile(While node)
        {
            Append("while(");
            node.Condition.Accept(this);
            Append(")");
            NewLine();
            Append("{");
            Indent();
            NewLine();
            node.Statement.Accept(this);
            Outdent();
            Append("}");
        }
    }
}
