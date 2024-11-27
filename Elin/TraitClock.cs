using System;

public class TraitClock : Trait
{
	public override void OnSimulateHour(VirtualDate date)
	{
		if (!date.IsRealTime || !this.owner.IsInstalled)
		{
			return;
		}
		if (this.owner.Cell.room == EClass.pc.Cell.room)
		{
			this.owner.PlaySound("clock_hour", 1f, true);
		}
	}
}
