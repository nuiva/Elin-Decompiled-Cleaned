using System;

public class GrowSystemKinoko : GrowSystem
{
	public override bool CanReapSeed()
	{
		return true;
	}

	protected override bool CanRegrow
	{
		get
		{
			return false;
		}
	}

	protected override bool WitherOnLastStage
	{
		get
		{
			return false;
		}
	}

	protected override bool UseGenericFirstStageTile
	{
		get
		{
			return false;
		}
	}

	public override bool NeedSunlight
	{
		get
		{
			return false;
		}
	}

	public override int StageLength
	{
		get
		{
			return 4;
		}
	}

	public override int AutoMineStage
	{
		get
		{
			return 0;
		}
	}

	public override void OnExceedLastStage()
	{
		base.SetStage(1, false);
	}

	public override void OnMineObj(Chara c = null)
	{
		base.PopHarvest(c ?? EClass.pc, ThingGen.CreateFromCategory("mushroom", -1), -1);
	}
}
