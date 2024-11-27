using System;

public class TraitTrapSister : TraitTrap
{
	public override bool CanDisarmTrap
	{
		get
		{
			return false;
		}
	}

	public override int DestroyChanceOnActivateTrap
	{
		get
		{
			return 100;
		}
	}
}
