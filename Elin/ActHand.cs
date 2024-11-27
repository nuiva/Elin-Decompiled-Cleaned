using System;

public class ActHand : Spell
{
	public override bool CanAutofire
	{
		get
		{
			return true;
		}
	}

	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override bool CanRapidFire
	{
		get
		{
			return true;
		}
	}
}
