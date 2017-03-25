using CodeConverter.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Reflection;

namespace CodeConverter.PowerShell
{
    public class CommandIntentFactory
    {
		public Intent DetermineCommandIntent(Invocation node)
		{
			var name = node.Expression as IdentifierName;
			if (name == null) return null;

			if (name.Name.Equals("Add-Content", StringComparison.OrdinalIgnoreCase))
			{
				return ProcessAddContent(node);
			}
			else if (name.Name.Equals("Get-Process", StringComparison.OrdinalIgnoreCase))
			{
				return ProcessGetProcess(node);
			}
			else if (name.Name.Equals("Get-Service", StringComparison.OrdinalIgnoreCase))
			{
				return ProcessGetService(node);
			}
			else if (name.Name.Equals("Out-File", StringComparison.OrdinalIgnoreCase))
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

		private Intent ProcessAddContent(Invocation node)
		{
			var filePath = GetParameter("Path", node);
			if (filePath == null) return null;
			var value = GetParameter("Value", node);
			if (value == null) return null;

			return new WriteFileIntent(node)
			{
				Append = new Argument("Append", null),
				Content = value.Expression,
				FilePath = filePath.Expression
			};
		}

		private Intent ProcessGetService(Invocation node)
		{
			var name = GetParameter("Name", node);

			return new GetServiceIntent(node)
			{
				Name = name
			};
		}

		private Intent ProcessGetProcess(Invocation node)
		{
			var name = GetParameter("Name", node);
			var id = GetParameter("Id", node);

			return new GetProcessIntent(node)
			{
				Name = name,
				Id = id
			};
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
    
    public class ParameterFinder
    {
        public static bool HasProxyCommand(CommandAst commandAst)
        {
            var commandName = commandAst.GetCommandName();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"CodeConverter.PowerShell.ProxyCommands.{commandName}.ps1";
            return assembly.GetManifestResourceStream(resourceName) != null;
        }

        public static Dictionary<string, object> FindBoundParameters(CommandAst commandAst)
        {
            var commandName = commandAst.GetCommandName();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CodeConverter.PowerShell.GetBoundParameters.ps1";

            string getBoundParameterScript;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                getBoundParameterScript = reader.ReadToEnd();
            }

            resourceName = $"CodeConverter.PowerShell.ProxyCommands.{commandName}.ps1";
            string proxyCommand;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                proxyCommand = reader.ReadToEnd();
            }

            var result = new Dictionary<string, object>();
            using (var powerShell = System.Management.Automation.PowerShell.Create())
            {
                powerShell.AddScript(getBoundParameterScript);
                powerShell.Invoke();
                powerShell.AddCommand("Get-BoundParameter");
                powerShell.AddParameter("commandAst", commandAst);
                powerShell.AddParameter("proxyCommand", proxyCommand);
                var psobject = powerShell.Invoke();

                foreach (var param in psobject.Select(m => m.BaseObject).Cast<Hashtable>())
                {
                    result.Add(param["Name"] as string, param["Ast"]);
                }
            }

            return result;
        }
    }
}
