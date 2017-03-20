using CodeConverter.Common;
using System;
using System.Linq;


namespace CodeConverter.PowerShell
{
    public class CommandIntentFactory
    {
		public Intent DetermineCommandIntent(Invocation node)
		{
			var name = node.Expression as IdentifierName;
			if (name == null) return null;

			if (name.Name.Equals("Out-File", StringComparison.OrdinalIgnoreCase))
			{
				return ProcessOutFile(node);
			}
			else if (name.Name.Equals("Start-Process", StringComparison.OrdinalIgnoreCase))
			{
				return ProcessStartProcess(node);
			}
			else if (name.Name.Equals("Write-Host", StringComparison.OrdinalIgnoreCase))
			{
				return ProcessWriteHost(node);
			}

			return null;
		}

		private Intent ProcessOutFile(Invocation node)
		{
			var filePath = GetParameter("FilePath", node);
			if (filePath == null) return null;
			var append = GetParameter("Append", node);
			var content = GetParameter("InputObject", node);
			if (content == null) return null;

			return new WriteFileIntent(node)
			{
				Append = append,
				Content = content.Expression,
				FilePath = filePath.Expression
			};
		}

		private Intent ProcessStartProcess(Invocation node)
		{
			var filePath = GetParameter("FilePath", node);
			if (filePath == null) return null;

			var argumentList = GetParameter("ArgumentList", node);
			if (argumentList == null) return null;

			return new StartProcessIntent(node)
			{
				FilePath = filePath,
				Arguments = argumentList
			};
		}

		private Intent ProcessWriteHost(Invocation node)
		{
			var obj = GetParameter("Object", node);
			if (obj == null) return null;

			return new WriteHostIntent(node)
			{
				Object = obj
			};
		}

		private Argument GetParameter(string name, Invocation node)
		{
			return node.Arguments.Arguments.Cast<Argument>().FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}
    }
}
