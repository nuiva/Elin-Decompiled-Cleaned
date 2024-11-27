using System;

public class ConDrunk : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			if (!this.owner._IsPC)
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
		this.owner.isDrunk = true;
	}

	public override void OnStart()
	{
		this.owner.ShowEmo(Emo.happy, 0f, true);
	}

	public override void Tick()
	{
		if (EClass.rnd(200) == 0 && this.GetPhase() >= 1)
		{
			this.owner.Vomit();
		}
		base.Mod(-1, false);
	}

	public override void OnRemoved()
	{
		this.owner.isDrunk = false;
	}
}
