using System;

public class AreaTypeWork : AreaType
{
	public override bool IsWork
	{
		get
		{
			return true;
		}
	}

	public override bool CanAssign
	{
		get
		{
			return true;
		}
	}

	public override bool IsPublicArea
	{
		get
		{
			return false;
		}
	}
}
