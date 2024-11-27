using System;

public class GlobalGoalVisitTown : GlobalGoalVisitAndStay
{
	public override Zone GetDestZone()
	{
		foreach (Zone zone in EClass.world.region.ListTowns().Shuffle<Zone>())
		{
			if (this.owner.currentZone != zone)
			{
				return zone;
			}
		}
		return base.GetDestZone();
	}
}
