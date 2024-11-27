using System;

public class TraitChangingRoom : TraitFloorSwitch
{
	public override bool UseAltTiles
	{
		get
		{
			return this.owner.ExistsOnMap && this.owner.pos.HasChara;
		}
	}

	public override void OnActivateTrap(Chara c)
	{
		c.PlaySound("Material/leather_drop", 1f, true);
		bool flag = c.pccData == null || c.pccData.state == PCCState.Normal;
		if (c.IsPCC)
		{
			c.SetPCCState(flag ? PCCState.Undie : PCCState.Normal);
		}
		c.Say(flag ? "cloth_remove" : "cloth_wear", c, null, null);
	}
}
