using System;

public class TraitResidentBoard : TraitBoard
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
		p.TrySetAct("LayerPeople", delegate()
		{
			EClass.ui.ToggleLayer<LayerPeople>(null);
			Tutorial.Play("work");
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
