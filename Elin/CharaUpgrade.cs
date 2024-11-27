using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CharaUpgrade : EClass
{
	public void Reset(Chara c)
	{
		foreach (CharaUpgrade.Item item in this.items)
		{
			Element element = c.elements.GetElement(item.idEle);
			if (element != null)
			{
				if (element is Feat)
				{
					c.SetFeat(item.idEle, 0, false);
				}
				else
				{
					c.elements.ModBase(item.idEle, -item.value);
				}
			}
		}
		this.items.Clear();
		c.feat += this.spent;
		this.spent = 0;
		this.count = 0;
		c.Refresh(false);
		this.reset++;
	}

	[JsonProperty]
	public List<CharaUpgrade.Item> items = new List<CharaUpgrade.Item>();

	[JsonProperty]
	public int count;

	[JsonProperty]
	public int spent;

	[JsonProperty]
	public int reset;

	[JsonProperty]
	public bool halt;

	public class Item : EClass
	{
		public int idEle
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

		public int value
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

		public int cost
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

		public DNA.Type type
		{
			get
			{
				return this.ints[3].ToEnum<DNA.Type>();
			}
			set
			{
				this.ints[3] = (int)value;
			}
		}

		[JsonProperty]
		public int[] ints = new int[4];
	}
}
