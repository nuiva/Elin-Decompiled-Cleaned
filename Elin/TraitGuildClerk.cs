using System;

public class TraitGuildClerk : TraitGuildPersonnel
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Guild;
		}
	}

	public override bool CanGuide
	{
		get
		{
			return true;
		}
	}
}
