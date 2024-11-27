using System;

public class ConSeeInvisible : BaseBuff
{
	public override bool SyncRide
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldRefresh
	{
		get
		{
			return true;
		}
	}

	public override void OnRefresh()
	{
		this.owner.canSeeInvisible = true;
	}
}
