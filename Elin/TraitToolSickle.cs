public class TraitToolSickle : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new TaskCullLife
		{
			dest = p.pos
		}, owner);
	}
}
