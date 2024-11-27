using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopTransaction : EClass
{
	public void Log()
	{
		foreach (ShopTransaction.Item item in this.bought)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Bought:",
				item.thing.Name,
				"/",
				item.num.ToString(),
				" x ",
				item.price.ToString(),
				" = ",
				(item.num * item.price).ToString()
			}));
		}
		foreach (ShopTransaction.Item item2 in this.sold)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Sold:",
				item2.thing.Name,
				"/",
				item2.num.ToString(),
				" x ",
				item2.price.ToString(),
				" = ",
				(item2.num * item2.price).ToString()
			}));
		}
	}

	public bool HasBought(Thing t)
	{
		using (List<ShopTransaction.Item>.Enumerator enumerator = this.bought.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.thing == t)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanSellBack(Thing t, int num = -1)
	{
		if (num == -1)
		{
			num = t.Num;
		}
		foreach (ShopTransaction.Item item in this.bought)
		{
			if (item.thing == t && item.num >= num)
			{
				return true;
			}
		}
		return false;
	}

	public void Process(Thing t, int n, bool sell)
	{
		int price = t.GetPrice(this.trader.currency, false, this.trader.priceType, null);
		int price2 = t.GetPrice(this.trader.currency, true, this.trader.priceType, null);
		int num = n;
		Debug.Log(string.Concat(new string[]
		{
			sell ? "■Selling:" : "■Buying:",
			t.Name,
			"/",
			n.ToString(),
			" x ",
			(sell ? price2 : price).ToString(),
			" = ",
			(n * (sell ? price2 : price)).ToString()
		}));
		int num2 = sell ? price : price2;
		List<ShopTransaction.Item> list = sell ? this.bought : this.sold;
		foreach (ShopTransaction.Item item in list)
		{
			if (item.thing.id == t.id && item.price == num2)
			{
				int num3 = (item.num >= num) ? num : item.num;
				num -= num3;
				item.num -= num3;
			}
			if (num == 0)
			{
				break;
			}
		}
		list.ForeachReverse(delegate(ShopTransaction.Item i)
		{
			if (i.num == 0)
			{
				list.Remove(i);
			}
		});
		if (num > 0)
		{
			if (sell)
			{
				this.sold.Add(new ShopTransaction.Item
				{
					thing = t,
					num = num,
					price = price2
				});
			}
			else
			{
				this.bought.Add(new ShopTransaction.Item
				{
					thing = t,
					num = num,
					price = price
				});
			}
		}
		this.Log();
	}

	public int GetPrice(Thing t, int n, bool sell)
	{
		int price = t.GetPrice(this.trader.currency, false, this.trader.priceType, null);
		int price2 = t.GetPrice(this.trader.currency, true, this.trader.priceType, null);
		int num = n;
		int num2 = 0;
		int num3 = sell ? price : price2;
		foreach (ShopTransaction.Item item in (sell ? this.bought : this.sold))
		{
			if (item.thing.id == t.id && item.price == num3)
			{
				int num4 = (item.num >= num) ? num : item.num;
				num -= num4;
				num2 += num4 * num3;
			}
			if (num == 0)
			{
				break;
			}
		}
		return num2 + num * (sell ? price2 : price);
	}

	public void OnEndTransaction()
	{
		Thing thing = this.trader.owner.things.Find("chest_merchant", -1, -1);
		if (thing != null)
		{
			thing.c_lockLv = 1;
		}
		foreach (ShopTransaction.Item item in this.bought)
		{
			Debug.Log(item.thing);
			item.thing.SetInt(101, 0);
			item.thing.isStolen = false;
		}
		if (this.trader.currency != CurrencyType.Money)
		{
			ShopTransaction.current = null;
			return;
		}
		int num = 0;
		foreach (ShopTransaction.Item item2 in this.bought)
		{
			for (int i = 0; i < item2.num; i++)
			{
				num += (int)Mathf.Sqrt((float)Mathf.Abs(item2.price * 5));
			}
			if (item2.thing.trait is TraitDeed)
			{
				EClass.player.flags.landDeedBought += item2.num;
			}
		}
		if (num >= 10)
		{
			EClass.pc.ModExp(291, num);
		}
		Debug.Log("negotiation total:" + num.ToString());
		num = 0;
		foreach (ShopTransaction.Item item3 in this.sold)
		{
			if (item3.thing.isStolen)
			{
				item3.thing.isStolen = false;
				for (int j = 0; j < item3.num; j++)
				{
					num += (int)Mathf.Sqrt((float)item3.price) / 2;
				}
			}
		}
		Debug.Log("stolen total:" + num.ToString());
		if (num > 0)
		{
			Guild.Thief.AddContribution(num);
		}
		ShopTransaction.current = null;
	}

	public static ShopTransaction current;

	public InvOwner trader;

	public List<ShopTransaction.Item> bought = new List<ShopTransaction.Item>();

	public List<ShopTransaction.Item> sold = new List<ShopTransaction.Item>();

	public class Item
	{
		public Thing thing;

		public int num;

		public int price;
	}
}
