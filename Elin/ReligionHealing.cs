using System;

public class ReligionHealing : Religion
{
	public override string id
	{
		get
		{
			return "healing";
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
