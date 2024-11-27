using System;

public class ConHallucination : BadCondition
{
	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		if (this.owner._IsPC)
		{
			Player.seedHallucination = EClass.rnd(10000) + 1;
		}
	}

	public override void OnRemoved()
	{
		if (this.owner._IsPC)
		{
			Player.seedHallucination = 0;
		}
	}
}
