using System.Linq;

public class GlobalGoalExplore : GlobalGoalVisitAndStay
{
	public override Zone GetDestZone()
	{
		foreach (Spatial item in EClass.world.region.children.Where((Spatial a) => (a as Zone).IsExplorable).ToList().Shuffle())
		{
			if (owner.currentZone != item)
			{
				return item as Zone;
			}
		}
		return base.GetDestZone();
	}
}
