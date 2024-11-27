using System;

public class ConTelepathy : BaseBuff
{
	public override bool ShouldRefresh
	{
		get
		{
			return true;
		}
	}

	public override void OnRefresh()
	{
		this.owner.hasTelepathy = true;
	}
}
