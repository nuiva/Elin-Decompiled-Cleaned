using System;

public class TraitFloorSwitch : TraitSwitch
{
	public override void OnStepped(Chara c)
	{
		if (this.IsNegativeEffect && EClass._zone.IsPCFaction && !c.IsHostile())
		{
			return;
		}
		this.owner.SetHidden(false);
		if (this.IgnoreWhenLevitating() && c.IsLevitating)
		{
			this.owner.Say("levitating", null, null);
			return;
		}
		if (this.CanDisarmTrap)
		{
			if (base.TryDisarmTrap(c))
			{
				return;
			}
			if (EClass.pc.Evalue(1656) >= 3)
			{
				return;
			}
		}
		base.ActivateTrap(c);
	}
}
