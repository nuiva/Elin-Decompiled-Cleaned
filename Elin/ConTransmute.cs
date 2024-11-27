using System;

public class ConTransmute : BaseBuff
{
	public override bool IsToggle
	{
		get
		{
			return true;
		}
	}

	public override void Tick()
	{
		if (this.owner.host == null)
		{
			base.Tick();
		}
	}

	public override void OnStart()
	{
		this.owner._CreateRenderer();
	}

	public override void OnRemoved()
	{
		this.owner._CreateRenderer();
	}
}
