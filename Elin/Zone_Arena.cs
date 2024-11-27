using System;

public class Zone_Arena : Zone
{
	public override bool RestrictBuild
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

	public override bool ScaleMonsterLevel
	{
		get
		{
			return base._dangerLv >= 51;
		}
	}

	public override bool MakeTownProperties
	{
		get
		{
			return true;
		}
	}

	public override bool UseFog
	{
		get
		{
			return true;
		}
	}
}
