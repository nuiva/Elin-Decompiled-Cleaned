public class ConParalyze : BadCondition
{
	public override Emo2 EmoIcon => Emo2.paralyzed;

	public override bool PreventRegen => true;

	public override bool ConsumeTurn => true;

	public override bool WillOverride => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.isParalyzed = true;
	}

	public override void OnRemoved()
	{
		owner.isParalyzed = false;
	}
}
