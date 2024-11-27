using System;
using System.Linq;

public class GlobalGoalExplore : GlobalGoalVisitAndStay
{
	public override Zone GetDestZone()
	{
		foreach (Spatial spatial in (from a in EClass.world.region.children
		where (a as Zone).IsExplorable
		select a).ToList<Spatial>().Shuffle<Spatial>())
		{
			if (this.owner.currentZone != spatial)
			{
				return spatial as Zone;
			}
		}
		return base.GetDestZone();
	}
}
