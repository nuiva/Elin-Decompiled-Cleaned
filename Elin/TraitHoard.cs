using System;

public class TraitHoard : Trait
{
	public override bool CanBeHeld
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			p.TrySetAct("actNewZone", delegate()
			{
				EClass.ui.AddLayer<LayerHoard>();
				return false;
			}, this.owner, CursorSystem.MoveZone, 1, false, true, false);
		}
	}
}
