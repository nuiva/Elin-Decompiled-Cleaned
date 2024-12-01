using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PopupManager : EClass
{
	public class Item
	{
		[JsonProperty]
		public string text;
	}

	[JsonProperty]
	public List<Item> items = new List<Item>();

	public WidgetPopup Instance => WidgetPopup.Instance;

	public void Add(string id)
	{
		string text = GameLang.Convert(Resources.Load<TextAsset>(CorePath.Text_Popup + id).text);
		Add(new Item
		{
			text = text
		});
	}

	public void Add(Item item)
	{
		items.Add(item);
		if (!EClass.debug.ignorePopup)
		{
			if (!Instance)
			{
				EClass.ui.widgets.ActivateWidget("Popup");
			}
			else
			{
				Instance.OnAddItem(item);
			}
			EClass.Sound.Play("popup_add");
		}
	}

	public void Remove(int index)
	{
	}
}
