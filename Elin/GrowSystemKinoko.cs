public class GrowSystemKinoko : GrowSystem
{
	protected override bool CanRegrow => false;

	protected override bool WitherOnLastStage => false;

	protected override bool UseGenericFirstStageTile => false;

	public override bool NeedSunlight => false;

	public override int StageLength => 4;

	public override int AutoMineStage => 0;

	public override bool CanReapSeed()
	{
		return true;
	}

	public override void OnExceedLastStage()
	{
		SetStage(1);
	}

	public override void OnMineObj(Chara c = null)
	{
		PopHarvest(c ?? EClass.pc, ThingGen.CreateFromCategory("mushroom"));
	}
}
