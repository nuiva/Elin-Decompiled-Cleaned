public class TraitHoard : Trait
{
	public override bool CanBeHeld => true;

	public override void TrySetAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			p.TrySetAct("actNewZone", delegate
			{
				EClass.ui.AddLayer<LayerHoard>();
				return false;
			}, owner, CursorSystem.MoveZone);
		}
	}
}
