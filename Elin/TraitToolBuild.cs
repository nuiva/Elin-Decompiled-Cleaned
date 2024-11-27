using System;

public class TraitToolBuild : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct("actBuildMode", delegate()
		{
			ActionMode.Inspect.Activate(true, false);
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
