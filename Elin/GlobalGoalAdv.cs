using System;

public class GlobalGoalAdv : GlobalGoal
{
	public override void OnAdvanceHour()
	{
		if (this.owner.isDead || this.owner.IsPCFaction || this.owner.currentZone == EClass._zone)
		{
			return;
		}
		if ((EClass.debug.enable && EClass.rnd(24) == 0) || (this.hours > 720 && EClass.rnd(720) == 0))
		{
			Zone randomTown = EClass.world.region.GetRandomTown();
			if (randomTown != this.owner.currentZone)
			{
				this.owner.MoveZone(randomTown, ZoneTransition.EnterState.RandomVisit);
			}
		}
	}
}
