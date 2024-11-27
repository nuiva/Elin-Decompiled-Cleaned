using System;

public class ConFaint : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.paralyzed;
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

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.owner.isFainted = true;
		if (this.owner.renderer != null)
		{
			this.owner.renderer.RefreshSprite();
		}
	}

	public override void OnRemoved()
	{
		this.owner.isFainted = false;
		this.owner.renderer.RefreshSprite();
		if (this.owner.IsPC)
		{
			Tutorial.Play("first");
		}
	}
}
