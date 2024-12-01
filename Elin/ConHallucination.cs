public class ConHallucination : BadCondition
{
	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		if (owner._IsPC)
		{
			Player.seedHallucination = EClass.rnd(10000) + 1;
		}
	}

	public override void OnRemoved()
	{
		if (owner._IsPC)
		{
			Player.seedHallucination = 0;
		}
	}
}
