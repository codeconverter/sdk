namespace CodeConverter.Common
{
    public class IntentVisitor
    {
		public virtual Node VisitWriteFileIntent(WriteFileIntent intent)
		{
			return intent.Node;
		}
    }
}
