namespace CodeConverter.Common
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
}
