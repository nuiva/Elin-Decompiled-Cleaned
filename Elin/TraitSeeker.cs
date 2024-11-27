using System;

public class TraitSeeker : TraitUniqueMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.RedBook;
		}
	}

	public override string LangBarter
	{
		get
		{
			return "daBorrowBook";
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.None;
		}
	}

	public override int RestockDay
	{
		get
		{
			return 360;
		}
	}
}
