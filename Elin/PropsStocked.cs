using System;

public class PropsStocked : Props
{
	public override PlaceState state
	{
		get
		{
			return PlaceState.stocked;
		}
	}

	public override bool IsStocked
	{
		get
		{
			return true;
		}
	}
}
