using System.Collections.Generic;
using UnityEngine.UI;

public class WidgetPopup : Widget
{
	public static WidgetPopup Instance;

	public LayoutGroup layout;

	public UIText textMain;

	public UIText textPage;

	public UIImage imageMain;

	public PopupManager.Item current;

	public PopupManager PM => EMono.player.popups;

	public List<PopupManager.Item> items => PM.items;

	public override void OnActivate()
	{
		Instance = this;
		if (items.Count == 0)
		{
			PM.Add("null");
		}
		Show();
	}

	public void OnAddItem(PopupManager.Item item)
	{
		Show();
	}

	public void Show()
	{
		Show(items.LastItem());
	}

	public void Show(PopupManager.Item item)
	{
		textMain.SetText(item.text);
		textPage.text = items.IndexOf(item) + 1 + " / " + items.Count;
		layout.RebuildLayout();
		current = item;
	}

	public void Next()
	{
		Show(items.NextItem(current));
	}

	public void Prev()
	{
		Show(items.PrevItem(current));
	}

	public void Discard()
	{
		items.Clear();
		Close();
	}

	public void AddItem()
	{
		this.RebuildLayout();
	}

	public void RemoveItem()
	{
	}
}
