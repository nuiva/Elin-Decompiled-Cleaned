public class TraitMiniPool : Trait
{
	public override void OnStepped(Chara c)
	{
		if (c.IsPC)
		{
			if (c.pccData.state == PCCState.Undie)
			{
				c.SetPCCState(PCCState.Normal);
				c.Say("cloth_wear", c);
			}
			else
			{
				c.SetPCCState(PCCState.Undie);
				c.Say("cloth_remove", c);
			}
			c.PlaySound("Material/leather_drop");
		}
	}
}
