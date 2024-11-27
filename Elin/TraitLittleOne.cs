using System;

public class TraitLittleOne : TraitChara
{
	public override bool CanBeTamed
	{
		get
		{
			return false;
		}
	}

	public override bool CanInvite
	{
		get
		{
			return false;
		}
	}

	public override bool IsCountAsResident
	{
		get
		{
			return true;
		}
	}
}
