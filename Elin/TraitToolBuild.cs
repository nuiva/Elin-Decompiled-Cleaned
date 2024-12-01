public class TraitToolBuild : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct("actBuildMode", delegate
		{
			ActionMode.Inspect.Activate();
			return false;
		}, owner);
	}
}
