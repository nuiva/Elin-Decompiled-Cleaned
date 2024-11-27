using System;

public class PropsRoaming : Props
{
	public override PlaceState state
	{
		get
		{
			return PlaceState.roaming;
		}
	}

	public override bool IsRoaming
	{
		get
		{
			return true;
		}
	}
}
