using System;

public class PropsInstalled : Props
{
	public override PlaceState state
	{
		get
		{
			return PlaceState.installed;
		}
	}

	public override bool IsInstalled
	{
		get
		{
			return true;
		}
	}
}
