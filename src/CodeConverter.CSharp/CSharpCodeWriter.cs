using CodeConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverter.CSharp
{
    public class CSharpCodeWriter : CStyleCodeWriter
    {
        public override Language Language => Language.CSharp;

        protected override Dictionary<BinaryOperator, string> OperatorMap => throw new NotImplementedException();

        public override void VisitMethodDeclaration(MethodDeclaration node)
        {
            Append("void ");
            Append(node.Name);
            Append("(");

            if (node.Parameters.Any())
            {
                foreach (var parameter in node.Parameters)
                {
                    parameter.Accept(this);
                    Append(", ");
                }
                Builder.Remove(Builder.Length - 2, 2);
            }

            Append(")");
            NewLine();
            Append("{");
            Indent();
            NewLine();

            node.Body.Accept(this);

            Outdent();
            Append("}");
        }
    }
}
