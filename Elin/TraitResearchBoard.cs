public class TraitResearchBoard : TraitBoard
{
	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone.IsPCFaction)
		{
			p.TrySetAct("actResearchBoard", delegate
			{
				EClass.ui.AddLayer<LayerTech>();
				return false;
			}, owner);
		}
	}
}
