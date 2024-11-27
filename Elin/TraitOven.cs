using System;

public class TraitOven : TraitCooker
{
	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}
}
