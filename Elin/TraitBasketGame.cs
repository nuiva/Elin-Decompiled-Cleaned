using System;

public class TraitBasketGame : TraitFloorSwitch
{
	public override void OnActivateTrap(Chara c)
	{
		if (!c.IsPC)
		{
			return;
		}
		MiniGame.Activate(MiniGame.Type.Basket);
	}
}
