using System;
using System.Collections.Generic;

public class ShadowData : EScriptable
{
	public static ShadowData Instance;

	public List<ShadowData.Item> items;

	[Serializable]
	public class Item
	{
		public int tile
		{
			get
			{
				return this.raw[0];
			}
			set
			{
				this.raw[0] = value;
			}
		}

		public int x
		{
			get
			{
				return this.raw[1];
			}
			set
			{
				this.raw[1] = value;
			}
		}

		public int y
		{
			get
			{
				return this.raw[2];
			}
			set
			{
				this.raw[2] = value;
			}
		}

		public int scaleX
		{
			get
			{
				return this.raw[3] + 100;
			}
			set
			{
				this.raw[3] = value - 100;
			}
		}

		public int scaleY
		{
			get
			{
				return this.raw[4] + 100;
			}
			set
			{
				this.raw[4] = value - 100;
			}
		}

		public int angle
		{
			get
			{
				return this.raw[5];
			}
			set
			{
				this.raw[5] = value;
			}
		}

		public override string ToString()
		{
			return "(" + this.tile.ToString() + ") " + this.name;
		}

		public void Validate()
		{
			if (this.raw.Length < 8)
			{
				Array.Resize<int>(ref this.raw, 8);
			}
		}

		public Item()
		{
			int[] array = new int[8];
			array[0] = 1;
			this.raw = array;
			base..ctor();
		}

		public string name;

		public int[] raw;
	}
}
