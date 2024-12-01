public class TraitBasketGame : TraitFloorSwitch
{
	public override void OnActivateTrap(Chara c)
	{
		if (c.IsPC)
		{
			MiniGame.Activate(MiniGame.Type.Basket);
		}
	}
}
