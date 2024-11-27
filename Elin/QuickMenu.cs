using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickMenu
{
	public void Build()
	{
		this.items.Clear();
		this.Add("Banner", 11, new Action(SE.Beep));
		this.Add("RemoveDesignation", 10, null);
		this.Add("Picker", 8, null);
		this.Add("StateEditor", 9, null);
		this.Add("EditArea", 3, null);
		this.Add("Inspect", 1, null);
		this.Add("Cut", 2, null);
		this.Add("Mine", 4, null);
		this.Add("Dig", 7, null);
		this.Add("DigFloor", 6, null);
	}

	public void Add(string id, int slot, Action action = null)
	{
		this.items.Add(new QuickMenu.Item
		{
			id = id,
			slot = slot,
			action = action
		});
	}

	public List<QuickMenu.Item> items = new List<QuickMenu.Item>();

	public class Item
	{
		public string id;

		public string text;

		public Sprite icon;

		public int slot;

		public Action action;
	}
}
