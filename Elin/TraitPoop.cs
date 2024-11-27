using System;

public class TraitPoop : TraitResourceMain
{
	public override int Decay
	{
		get
		{
			return 10;
		}
	}

	public override int DecaySpeed
	{
		get
		{
			return base.DecaySpeed;
		}
	}
}
