public class ConConfuse : BadCondition
{
	public override Emo2 EmoIcon => Emo2.confused;

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.isConfused = true;
	}

	public override void OnRemoved()
	{
		owner.isConfused = false;
	}
}
