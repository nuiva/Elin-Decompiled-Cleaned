using System;

public class TraitBuildBoard : TraitBoard
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.godBuild && !EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("actBuildMode", delegate()
		{
			BuildMenu.Toggle();
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
