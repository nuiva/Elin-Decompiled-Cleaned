using System;

public class TraitInformer : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 15;
		}
	}

	public override bool CanPicklock
	{
		get
		{
			return true;
		}
	}

	public override bool HaveNews
	{
		get
		{
			return true;
		}
	}
}
