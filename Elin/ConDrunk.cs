public class ConDrunk : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			if (!owner._IsPC)
			{
				return Emo2.happy;
			}
			return Emo2.none;
		}
	}

	public override int GetPhase()
	{
		if (base.value < 50)
		{
			return 0;
		}
		return 1;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, onDeserialize);
		owner.isDrunk = true;
	}

	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
	}

	public override void Tick()
	{
		if (EClass.rnd(200) == 0 && GetPhase() >= 1)
		{
			owner.Vomit();
		}
		Mod(-1);
	}

	public override void OnRemoved()
	{
		owner.isDrunk = false;
	}
}
