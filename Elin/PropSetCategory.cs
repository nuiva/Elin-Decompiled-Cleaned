public class PropSetCategory : PropSet
{
	public SourceCategory.Row source;

	public PropSetCategory parent;

	public int sum;

	public override void ModNum(int a)
	{
		num += a;
		ModSum(a);
	}

	public void ModSum(int a)
	{
		if (parent != null)
		{
			parent.ModSum(a);
		}
		sum += a;
	}
}
