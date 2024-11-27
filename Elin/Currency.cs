using System;

public class Currency
{
	public static string ToID(CurrencyType currency)
	{
		switch (currency)
		{
		case CurrencyType.Medal:
			return "medal";
		case CurrencyType.Plat:
			return "plat";
		case CurrencyType.Ecopo:
			return "ecopo";
		case CurrencyType.Money2:
			return "money2";
		case CurrencyType.Influence:
			return "influence";
		case CurrencyType.Casino_coin:
			return "casino_coin";
		}
		return "money";
	}
}
