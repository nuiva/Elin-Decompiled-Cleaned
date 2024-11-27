using System;

public class TraitMiniPool : Trait
{
	public override void OnStepped(Chara c)
	{
		if (!c.IsPC)
		{
			return;
		}
		if (c.pccData.state == PCCState.Undie)
		{
			c.SetPCCState(PCCState.Normal);
			c.Say("cloth_wear", c, null, null);
		}
		else
		{
			c.SetPCCState(PCCState.Undie);
			c.Say("cloth_remove", c, null, null);
		}
		c.PlaySound("Material/leather_drop", 1f, true);
	}
}
