using System;

public class ConConfuse : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.confused;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.owner.isConfused = true;
	}

	public override void OnRemoved()
	{
		this.owner.isConfused = false;
	}
}
