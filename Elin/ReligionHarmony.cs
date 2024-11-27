using System;

public class ReligionHarmony : Religion
{
	public override string id
	{
		get
		{
			return "harmony";
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
