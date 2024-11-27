using System;

public class ConParalyze : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.paralyzed;
		}
	}

	public override bool PreventRegen
	{
		get
		{
			return true;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override bool ConsumeTurn
	{
		get
		{
			return true;
		}
	}

	public override bool WillOverride
	{
		get
		{
			return true;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.owner.isParalyzed = true;
	}

	public override void OnRemoved()
	{
		this.owner.isParalyzed = false;
	}
}
