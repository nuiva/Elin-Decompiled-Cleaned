using System;

public class TraitTool : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool IsTool
	{
		get
		{
			return true;
		}
	}

	public override bool ShowAsTool
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
	}
}
