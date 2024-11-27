using System;

public class TraitWorkbenchFuel : TraitWorkbench
{
	public override int MaxFuel
	{
		get
		{
			return 200;
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}
}
