using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMultiList : MonoBehaviour
{
	public void Clear()
	{
		this.owners.Clear();
	}

	public void AddOwner(int i, ListOwner o)
	{
		o.index = this.owners.Count;
		this.owners.Add(o);
		o.layer = this.layer;
		o.window = this.windows[i];
		o.list = this.lists[i];
		o.multi = this;
		o.main = (i == 0);
		if (!this.Double && this.addTab)
		{
			o.window.AddTab(o.TextTab, null, null, null, null);
		}
		o.OnCreate();
	}

	public void Build(UIList.SortMode m = UIList.SortMode.ByNone)
	{
		foreach (ListOwner listOwner in this.owners)
		{
			listOwner.list.sortMode = m;
		}
		if (this.Double)
		{
			this.owners[0].other = this.owners[1];
			this.owners[1].other = this.owners[0];
		}
	}

	public void Refresh()
	{
		foreach (ListOwner listOwner in this.owners)
		{
			listOwner.OnSwitchContent();
		}
	}

	public List<ListOwner> owners = new List<ListOwner>();

	public Layer layer;

	public Window[] windows;

	public UIList[] lists;

	public UIHeader[] headers;

	public bool Double;

	public bool addTab;
}
