using System;

public class Zone_Tent : Zone
{
	public override bool PetFollow
	{
		get
		{
			return false;
		}
	}

	public override bool AllowNewZone
	{
		get
		{
			return false;
		}
	}

	public override bool UseFog
	{
		get
		{
			return true;
		}
	}

	public override bool GrowPlant
	{
		get
		{
			return true;
		}
	}

	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Bottom;
		}
	}

	public override void OnBeforeDeactivate()
	{
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			num += thing.ChildrenAndSelfWeight;
		}
		base.SetInt(1, num);
	}
}
