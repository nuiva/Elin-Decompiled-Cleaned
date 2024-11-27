using System;

public class TraitHomeBoard : TraitBoard
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
		if (!EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("actLayerHome", delegate()
		{
			EClass.ui.ToggleLayer<LayerHome>(null);
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
