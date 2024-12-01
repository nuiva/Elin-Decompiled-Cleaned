using System.Collections.Generic;
using Newtonsoft.Json;

public class ShippingResult : EClass
{
	public class Item
	{
		[JsonProperty]
		public string[] _strs = new string[3];

		public string text
		{
			get
			{
				return _strs[0];
			}
			set
			{
				_strs[0] = value;
			}
		}

		public int income
		{
			get
			{
				return _strs[1].ToInt();
			}
			set
			{
				_strs[1] = value.ToString() ?? "";
			}
		}
	}

	[JsonProperty]
	public int[] ints = new int[10];

	[JsonProperty]
	public List<Item> items = new List<Item>();

	public int total
	{
		get
		{
			return ints[0];
		}
		set
		{
			ints[0] = value;
		}
	}

	public int rawDate
	{
		get
		{
			return ints[1];
		}
		set
		{
			ints[1] = value;
		}
	}

	public int uidZone
	{
		get
		{
			return ints[2];
		}
		set
		{
			ints[2] = value;
		}
	}

	public int hearthLv
	{
		get
		{
			return ints[3];
		}
		set
		{
			ints[3] = value;
		}
	}

	public int hearthExp
	{
		get
		{
			return ints[4];
		}
		set
		{
			ints[4] = value;
		}
	}

	public int hearthExpGained
	{
		get
		{
			return ints[5];
		}
		set
		{
			ints[5] = value;
		}
	}

	public int debt
	{
		get
		{
			return ints[6];
		}
		set
		{
			ints[6] = value;
		}
	}

	public int GetIncome()
	{
		int num = 0;
		foreach (Item item in items)
		{
			num += item.income;
		}
		return num;
	}
}
