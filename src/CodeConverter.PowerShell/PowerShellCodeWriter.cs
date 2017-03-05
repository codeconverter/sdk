using CodeConverter.Common;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverter.PowerShell
{
    public class PowerShellCodeWriter : CStyleCodeWriter
    {
        private static Dictionary<BinaryOperator, string> _operatorMap;
        protected override Dictionary<BinaryOperator, string> OperatorMap => _operatorMap;
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

        public override void VisitArrayCreation(ArrayCreation node)
        {
            Append("@(");
            foreach(var item in node.Initializer)
            {
                item.Accept(this);
                Append(",");
            }

            // Remove last ,
            Builder.Remove(Builder.Length - 1, 1);
            Append(")");
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

        public override void VisitCast(Cast node)
        {
            Append("[");
            Append(node.Type);
            Append("]");
            node.Expression.Accept(this);
        }

        public override void VisitCatchDeclaration(CatchDeclaration node)
        {
            Append("[");
            Append(node.Type);
            Append("]");
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

        public override void VisitLiteral(Literal node)
        {
            if (node.Token == "true" || node.Token == "false")
                Append("$");

            Append(node.Token);
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

        public override void VisitStringConstant(StringConstant node)
        {
            Append("\'" + node.Value + "\'");
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
    }
}
