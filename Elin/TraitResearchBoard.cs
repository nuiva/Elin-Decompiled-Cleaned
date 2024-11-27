using System;

public class TraitResearchBoard : TraitBoard
{
	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("actResearchBoard", delegate()
		{
			EClass.ui.AddLayer<LayerTech>();
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
