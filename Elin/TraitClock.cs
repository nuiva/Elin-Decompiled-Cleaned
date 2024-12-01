public class TraitClock : Trait
{
	public override void OnSimulateHour(VirtualDate date)
	{
		if (date.IsRealTime && owner.IsInstalled && owner.Cell.room == EClass.pc.Cell.room)
		{
			owner.PlaySound("clock_hour");
		}
	}
}
