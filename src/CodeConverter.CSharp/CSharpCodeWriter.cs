using CodeConverter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConverter.CSharp
{
    public class CSharpCodeWriter : CodeWriter
    {
        public override Language Language => Language.CSharp;

        public override void VisitMethodDeclaration(MethodDeclaration node)
        {
        }
    }
}
