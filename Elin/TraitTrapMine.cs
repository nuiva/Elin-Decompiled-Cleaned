using System;

public class TraitTrapMine : TraitTrap
{
	public override int DestroyChanceOnActivateTrap
	{
		get
		{
			return 100;
		}
	}

	public override void OnStepped(Chara c)
	{
		if (c.id == "dog_mine")
		{
			return;
		}
		base.OnStepped(c);
	}
}
