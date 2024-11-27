using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class PopupManager : EClass
{
	public WidgetPopup Instance
	{
		get
		{
			return WidgetPopup.Instance;
		}
	}

	public void Add(string id)
	{
		string text = GameLang.Convert(Resources.Load<TextAsset>(CorePath.Text_Popup + id).text);
		this.Add(new PopupManager.Item
		{
			text = text
		});
	}

	public void Add(PopupManager.Item item)
	{
		this.items.Add(item);
		if (EClass.debug.ignorePopup)
		{
			return;
		}
		if (!this.Instance)
		{
			EClass.ui.widgets.ActivateWidget("Popup");
		}
		else
		{
			this.Instance.OnAddItem(item);
		}
		EClass.Sound.Play("popup_add");
	}

	public void Remove(int index)
	{
	}

	[JsonProperty]
	public List<PopupManager.Item> items = new List<PopupManager.Item>();

	public class Item
	{
		[JsonProperty]
		public string text;
	}
}
