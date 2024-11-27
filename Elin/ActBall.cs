using System;

public class ActBall : Spell
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

	public override float RapidDelay
	{
		get
		{
			return 0.3f;
		}
	}
}
