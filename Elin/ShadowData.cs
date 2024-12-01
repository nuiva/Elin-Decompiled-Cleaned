using System;
using System.Collections.Generic;

public class ShadowData : EScriptable
{
	[Serializable]
	public class Item
	{
		public string name;

		public int[] raw = new int[8] { 1, 0, 0, 0, 0, 0, 0, 0 };

		public int tile
		{
			get
			{
				return raw[0];
			}
			set
			{
				raw[0] = value;
			}
		}

		public int x
		{
			get
			{
				return raw[1];
			}
			set
			{
				raw[1] = value;
			}
		}

		public int y
		{
			get
			{
				return raw[2];
			}
			set
			{
				raw[2] = value;
			}
		}

		public int scaleX
		{
			get
			{
				return raw[3] + 100;
			}
			set
			{
				raw[3] = value - 100;
			}
		}

		public int scaleY
		{
			get
			{
				return raw[4] + 100;
			}
			set
			{
				raw[4] = value - 100;
			}
		}

		public int angle
		{
			get
			{
				return raw[5];
			}
			set
			{
				raw[5] = value;
			}
		}

		public override string ToString()
		{
			return "(" + tile + ") " + name;
		}

		public void Validate()
		{
			if (raw.Length < 8)
			{
				Array.Resize(ref raw, 8);
			}
		}
	}

	public static ShadowData Instance;

	public List<Item> items;
}
