public class TraitHomeBoard : TraitBoard
{
	public override bool IsHomeItem => true;

	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone.IsPCFaction)
		{
			p.TrySetAct("actLayerHome", delegate
			{
				EClass.ui.ToggleLayer<LayerHome>();
				return false;
			}, owner);
		}
	}
}
