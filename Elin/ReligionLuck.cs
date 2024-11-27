using System;

public class ReligionLuck : Religion
{
	public override string id
	{
		get
		{
			return "luck";
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
