using System;

public class ReligionOblivion : Religion
{
	public override string id
	{
		get
		{
			return "oblivion";
		}
	}

	public override bool CanJoin
	{
		get
		{
			return false;
		}
	}
}
