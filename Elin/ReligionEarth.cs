using System;

public class ReligionEarth : Religion
{
	public override string id
	{
		get
		{
			return "earth";
		}
	}

	public override bool IsAvailable
	{
		get
		{
			return true;
		}
	}

	public override void OnBecomeBranchFaith()
	{
	}
}
