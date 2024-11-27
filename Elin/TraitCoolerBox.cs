using System;

public class TraitCoolerBox : TraitContainer
{
	public override bool IsFridge
	{
		get
		{
			return true;
		}
	}

	public override int DecaySpeedChild
	{
		get
		{
			return 10;
		}
	}
}
