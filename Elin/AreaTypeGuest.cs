using System;

public class AreaTypeGuest : AreaType
{
	public override bool IsPublicArea
	{
		get
		{
			return false;
		}
	}
}
