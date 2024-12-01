public class ConBlind : BadCondition
{
	public override Emo2 EmoIcon => Emo2.blind;

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.isBlind = true;
	}

	public override void OnStart()
	{
		if (owner.IsPC)
		{
			owner.RecalculateFOV();
			ScreenGrading.blind = true;
			EClass.scene.camSupport.grading.SetGrading();
		}
	}

	public override void OnRemoved()
	{
		owner.isBlind = false;
		if (owner.IsPC)
		{
			owner.RecalculateFOV();
			ScreenGrading.blind = false;
			EClass.scene.camSupport.grading.SetGrading();
		}
	}
}
