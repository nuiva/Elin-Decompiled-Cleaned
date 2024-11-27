using System;

public class TraitUniqueGuildPersonnel : TraitGuildPersonnel
{
	public override bool IsUnique
	{
		get
		{
			return true;
		}
	}
}
