public class TraitBuildBoard : TraitBoard
{
	public override bool IsHomeItem => true;

	public override void TrySetAct(ActPlan p)
	{
		if (EClass.debug.godBuild || EClass._zone.IsPCFaction)
		{
			p.TrySetAct("actBuildMode", delegate
			{
				BuildMenu.Toggle();
				return false;
			}, owner);
		}
	}
}
