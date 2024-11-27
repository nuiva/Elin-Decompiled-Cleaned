using System;

public class ActBreathe : Ability
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
