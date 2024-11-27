using System;

public class TraitFridge : TraitContainer
{
	public override bool IsFridge
	{
		get
		{
			return true;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override int DecaySpeedChild
	{
		get
		{
			if (!this.owner.isOn)
			{
				return base.DecaySpeedChild;
			}
			return 0;
		}
	}
}
