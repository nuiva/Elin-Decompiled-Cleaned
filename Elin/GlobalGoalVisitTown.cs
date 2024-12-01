public class GlobalGoalVisitTown : GlobalGoalVisitAndStay
{
	public override Zone GetDestZone()
	{
		foreach (Zone item in EClass.world.region.ListTowns().Shuffle())
		{
			if (owner.currentZone != item)
			{
				return item;
			}
		}
		return base.GetDestZone();
	}
}
