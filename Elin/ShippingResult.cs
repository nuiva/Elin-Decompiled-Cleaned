using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ShippingResult : EClass
{
	public int total
	{
		get
		{
			return this.ints[0];
		}
		set
		{
			this.ints[0] = value;
		}
	}

	public int rawDate
	{
		get
		{
			return this.ints[1];
		}
		set
		{
			this.ints[1] = value;
		}
	}

	public int uidZone
	{
		get
		{
			return this.ints[2];
		}
		set
		{
			this.ints[2] = value;
		}
	}

	public int hearthLv
	{
		get
		{
			return this.ints[3];
		}
		set
		{
			this.ints[3] = value;
		}
	}

	public int hearthExp
	{
		get
		{
			return this.ints[4];
		}
		set
		{
			this.ints[4] = value;
		}
	}

	public int hearthExpGained
	{
		get
		{
			return this.ints[5];
		}
		set
		{
			this.ints[5] = value;
		}
	}

	public int debt
	{
		get
		{
			return this.ints[6];
		}
		set
		{
			this.ints[6] = value;
		}
	}

	public int GetIncome()
	{
		int num = 0;
		foreach (ShippingResult.Item item in this.items)
		{
			num += item.income;
		}
		return num;
	}

	[JsonProperty]
	public int[] ints = new int[10];

	[JsonProperty]
	public List<ShippingResult.Item> items = new List<ShippingResult.Item>();

	public class Item
	{
		public string text
		{
			get
			{
				return this._strs[0];
			}
			set
			{
				this._strs[0] = value;
			}
		}

		public int income
		{
			get
			{
				return this._strs[1].ToInt();
			}
			set
			{
				this._strs[1] = (value.ToString() ?? "");
			}
		}

		[JsonProperty]
		public string[] _strs = new string[3];
	}
}
