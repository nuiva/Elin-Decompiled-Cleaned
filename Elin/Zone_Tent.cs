public class Zone_Tent : Zone
{
	public override bool PetFollow => false;

	public override bool AllowNewZone => false;

	public override bool UseFog => true;

	public override bool GrowPlant => true;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;

	public override void OnBeforeDeactivate()
	{
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			num += thing.ChildrenAndSelfWeight;
		}
		SetInt(1, num);
	}
}
