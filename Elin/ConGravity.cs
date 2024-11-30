using System;

public class ConGravity : BaseDebuff
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
		this.owner._isLevitating = false;
		if (EClass.core.IsGameStarted)
		{
			this.owner.RefreshSpeed(null);
		}
	}

	public override int GetPhase()
	{
		return 0;
	}
}
