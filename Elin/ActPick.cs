public class ActPick : Act
{
	public override bool LocalAct => false;

	public override CursorInfo CursorIcon => CursorSystem.Inventory;

	public override bool CanPerform()
	{
		if (Act.TP == null || Act.TP.detail == null || Act.TP.detail.things.Count == 0)
		{
			return false;
		}
		return true;
	}

	public override bool Perform()
	{
		foreach (Card item in Act.TP.ListCards())
		{
			if (item.isThing && item.placeState == PlaceState.roaming)
			{
				Act.CC.Pick(item.Thing);
			}
		}
		return true;
	}
}
