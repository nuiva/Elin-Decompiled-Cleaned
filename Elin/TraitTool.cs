public class TraitTool : Trait
{
	public override bool CanStack => false;

	public override bool IsTool => true;

	public override bool ShowAsTool => true;

	public override void TrySetAct(ActPlan p)
	{
	}
}
