using System;

public class Zone_Civilized : Zone
{
	public override bool ShouldRegenerate
	{
		get
		{
			return true;
		}
	}

	public override bool HasLaw
	{
		get
		{
			return true;
		}
	}

	public override bool AllowCriminal
	{
		get
		{
			return true;
		}
	}
}
