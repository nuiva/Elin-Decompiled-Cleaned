using System;

public class PropSetCategory : PropSet
{
	public override void ModNum(int a)
	{
		this.num += a;
		this.ModSum(a);
	}

	public void ModSum(int a)
	{
		if (this.parent != null)
		{
			this.parent.ModSum(a);
		}
		this.sum += a;
	}

	public SourceCategory.Row source;

	public PropSetCategory parent;

	public int sum;
}
