using CodeConverter.Common;
using System.Collections.Generic;

namespace CodeConverter.CSharp
{
	public class CSharpIntentVisitor : IntentVisitor
    {
		public override Node VisitWriteFileIntent(WriteFileIntent intent)
		{
			var fileClass = new IdentifierName("File");
			var method = "WriteAllText";
			var arguments = new List<Argument>();

			if (intent.Append != null)
			{
				method = "AppendAllText";
			}

			var memberAccess = new MemberAccess(fileClass, method);

			arguments.Add(new Argument(intent.FilePath));
			arguments.Add(new Argument(intent.Content));

			return new Invocation(memberAccess, new ArgumentList(arguments));
		}
	}
}
