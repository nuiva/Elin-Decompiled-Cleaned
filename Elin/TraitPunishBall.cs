using System;

public class TraitPunishBall : Trait
{
	public override bool CanBeDropped
	{
		get
		{
			return false;
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeStolen
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return EClass.debug.enable;
		}
	}
}
