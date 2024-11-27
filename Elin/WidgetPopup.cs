using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class WidgetPopup : Widget
{
	public PopupManager PM
	{
		get
		{
			return EMono.player.popups;
		}
	}

	public List<PopupManager.Item> items
	{
		get
		{
			return this.PM.items;
		}
	}

	public override void OnActivate()
	{
		WidgetPopup.Instance = this;
		if (this.items.Count == 0)
		{
			this.PM.Add("null");
		}
		this.Show();
	}

	public void OnAddItem(PopupManager.Item item)
	{
		this.Show();
	}

	public void Show()
	{
		this.Show(this.items.LastItem<PopupManager.Item>());
	}

	public void Show(PopupManager.Item item)
	{
		this.textMain.SetText(item.text);
		this.textPage.text = (this.items.IndexOf(item) + 1).ToString() + " / " + this.items.Count.ToString();
		this.layout.RebuildLayout(false);
		this.current = item;
	}

	public void Next()
	{
		this.Show(this.items.NextItem(this.current));
	}

	public void Prev()
	{
		this.Show(this.items.PrevItem(this.current));
	}

	public void Discard()
	{
		this.items.Clear();
		base.Close();
	}

	public void AddItem()
	{
		this.RebuildLayout(false);
	}

	public void RemoveItem()
	{
	}

	public static WidgetPopup Instance;

	public LayoutGroup layout;

	public UIText textMain;

	public UIText textPage;

	public UIImage imageMain;

	public PopupManager.Item current;
}
