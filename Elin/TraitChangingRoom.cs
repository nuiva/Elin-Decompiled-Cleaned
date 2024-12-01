public class TraitChangingRoom : TraitFloorSwitch
{
	public override bool UseAltTiles
	{
		get
		{
			if (owner.ExistsOnMap)
			{
				return owner.pos.HasChara;
			}
			return false;
		}
	}

	public override void OnActivateTrap(Chara c)
	{
		c.PlaySound("Material/leather_drop");
		bool flag = c.pccData == null || c.pccData.state == PCCState.Normal;
		if (c.IsPCC)
		{
			c.SetPCCState(flag ? PCCState.Undie : PCCState.Normal);
		}
		c.Say(flag ? "cloth_remove" : "cloth_wear", c);
	}
}
