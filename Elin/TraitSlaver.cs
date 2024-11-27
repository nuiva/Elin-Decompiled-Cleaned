using System;

public class TraitSlaver : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 15;
		}
	}

	public override string IDRumor
	{
		get
		{
			return "slaver";
		}
	}

	public override SlaverType SlaverType
	{
		get
		{
			return SlaverType.Slave;
		}
	}
}
