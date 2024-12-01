using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickMenu
{
	public class Item
	{
		public string id;

		public string text;

		public Sprite icon;

		public int slot;

		public Action action;
	}

	public List<Item> items = new List<Item>();

	public void Build()
	{
		items.Clear();
		Add("Banner", 11, SE.Beep);
		Add("RemoveDesignation", 10);
		Add("Picker", 8);
		Add("StateEditor", 9);
		Add("EditArea", 3);
		Add("Inspect", 1);
		Add("Cut", 2);
		Add("Mine", 4);
		Add("Dig", 7);
		Add("DigFloor", 6);
	}

	public void Add(string id, int slot, Action action = null)
	{
		items.Add(new Item
		{
			id = id,
			slot = slot,
			action = action
		});
	}
}
