using System;

public class ConLevitate : BaseBuff
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
		this.owner._isLevitating = true;
	}
}
