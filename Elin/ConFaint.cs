public class ConFaint : BadCondition
{
	public override Emo2 EmoIcon => Emo2.paralyzed;

	public override bool ConsumeTurn => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.isFainted = true;
		if (owner.renderer != null)
		{
			owner.renderer.RefreshSprite();
		}
	}

	public override void OnRemoved()
	{
		owner.isFainted = false;
		owner.renderer.RefreshSprite();
		if (owner.IsPC)
		{
			Tutorial.Play("first");
		}
	}
}
