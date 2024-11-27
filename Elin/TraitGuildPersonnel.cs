using System;

public class TraitGuildPersonnel : TraitCitizen
{
	public override bool CanInvite
	{
		get
		{
			return false;
		}
	}
}
