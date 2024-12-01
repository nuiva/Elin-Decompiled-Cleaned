using System.Collections.Generic;

public class ActList : EClass
{
	public class Item
	{
		public Act act;

		public int chance;

		public bool pt;
	}

	public List<Item> items = new List<Item>();
}
