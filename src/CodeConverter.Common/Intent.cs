﻿namespace CodeConverter.Common
{
	public abstract class Intent
    {
		public Intent(Node node)
		{
			Node = node;
		}
		public Node Node { get; }
		public abstract Node Accept(IntentVisitor visitor);
	}

	public class StartProcessIntent : Intent
	{
		public StartProcessIntent(Node node) : base(node)
		{
		}

		public Node FilePath { get; set; }
		public Node Arguments { get; set; }

		public override Node Accept(IntentVisitor visitor)
		{
			return visitor.VisitStartProcessIntent(this);
		}
	}

	public class GetProcessIntent : Intent
	{
		public GetProcessIntent(Node node) : base(node)
		{
		}

		public Node Name { get; set; }
		public Node Id { get; set; }
		public override Node Accept(IntentVisitor visitor)
		{
			return visitor.VisitGetProcessIntent(this);
		}
	}

	public class GetServiceIntent : Intent
	{
		public GetServiceIntent(Node node) : base(node)
		{
		}

		public Node Name { get; set; }
		public override Node Accept(IntentVisitor visitor)
		{
			return visitor.VisitGetServiceIntent(this);
		}
	}

	public class WriteFileIntent : Intent
	{
		public WriteFileIntent(Node node) : base(node)
		{
		}

		public Node FilePath { get; set; }
		public Node Append { get; set; }
		public Node Content { get; set; }

		public override Node Accept(IntentVisitor visitor)
		{
			return visitor.VisitWriteFileIntent(this);
		}
	}

	public class WriteHostIntent : Intent
	{
		public WriteHostIntent(Node node) : base(node)
		{
		}

		public Node Object { get; set; }

		public override Node Accept(IntentVisitor visitor)
		{
			return visitor.VisitWriteHostIntent(this);
		}
	}
}
