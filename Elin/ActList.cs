using System;
using System.Collections.Generic;

public class ActList : EClass
{
	public List<ActList.Item> items = new List<ActList.Item>();

	public class Item
	{
		public Act act;

		public int chance;

		public bool pt;
	}
}
