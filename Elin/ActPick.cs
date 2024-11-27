using System;

public class ActPick : Act
{
	public override bool LocalAct
	{
		get
		{
			return false;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Inventory;
		}
	}

	public override bool CanPerform()
	{
		return Act.TP != null && Act.TP.detail != null && Act.TP.detail.things.Count != 0;
	}

	public override bool Perform()
	{
		foreach (Card card in Act.TP.ListCards(false))
		{
			if (card.isThing && card.placeState == PlaceState.roaming)
			{
				Act.CC.Pick(card.Thing, true, true);
			}
		}
		return true;
	}
}
