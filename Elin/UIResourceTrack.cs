using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceTrack : EMono
{
	public static void Refresh()
	{
		if (UIResourceTrack.Instance)
		{
			UIResourceTrack.Instance._Refresh();
		}
	}

	public void OnActivate()
	{
		UIResourceTrack.Instance = this;
		this.buttonAdd.SetOnClick(delegate
		{
			EMono.ui.AddLayer<LayerResource>();
		});
		if (!this.mold)
		{
			this.mold = this.layout.CreateMold(null);
		}
		this._Refresh();
	}

	public void OnMoveZone()
	{
		this.dictProp.Clear();
		this.dictCat.Clear();
		this.layout.DestroyChildren(false, true);
		this._Refresh();
	}

	public void _Refresh()
	{
		this.dirtyLayout = false;
		HashSet<string> trackedCards = EMono.player.trackedCards;
		HashSet<string> trackedCategories = EMono.player.trackedCategories;
		foreach (string text in trackedCards)
		{
			if (this.dictProp.ContainsKey(text))
			{
				this.dictProp[text].Refresh();
			}
			else
			{
				ButtonResourceTrack buttonResourceTrack = Util.Instantiate<ButtonResourceTrack>(this.mold, this.layout);
				buttonResourceTrack.SetProp(text);
				this.dictProp.Add(text, buttonResourceTrack);
				this.dirtyLayout = true;
			}
		}
		foreach (string text2 in trackedCategories)
		{
			if (this.dictCat.ContainsKey(text2))
			{
				this.dictCat[text2].Refresh();
			}
			else
			{
				ButtonResourceTrack buttonResourceTrack2 = Util.Instantiate<ButtonResourceTrack>(this.mold, this.layout);
				buttonResourceTrack2.SetCat(text2);
				this.dictCat.Add(text2, buttonResourceTrack2);
				this.dirtyLayout = true;
			}
		}
		this.listRemove.Clear();
		foreach (string item in this.dictProp.Keys)
		{
			if (!trackedCards.Contains(item))
			{
				this.listRemove.Add(item);
			}
		}
		foreach (string key in this.listRemove)
		{
			UnityEngine.Object.DestroyImmediate(this.dictProp[key].gameObject);
			this.dictProp.Remove(key);
			this.dirtyLayout = true;
		}
		this.listRemove.Clear();
		foreach (string item2 in this.dictCat.Keys)
		{
			if (!trackedCategories.Contains(item2))
			{
				this.listRemove.Add(item2);
			}
		}
		foreach (string key2 in this.listRemove)
		{
			UnityEngine.Object.DestroyImmediate(this.dictCat[key2].gameObject);
			this.dictCat.Remove(key2);
			this.dirtyLayout = true;
		}
		if (this.dirtyLayout)
		{
			this.buttonAdd.transform.SetAsLastSibling();
			this.layout.RebuildLayout(true);
		}
	}

	public static UIResourceTrack Instance;

	public LayoutGroup layout;

	public ButtonResourceTrack mold;

	public UIButton buttonAdd;

	public Dictionary<string, ButtonResourceTrack> dictProp = new Dictionary<string, ButtonResourceTrack>();

	public Dictionary<string, ButtonResourceTrack> dictCat = new Dictionary<string, ButtonResourceTrack>();

	private List<string> listRemove = new List<string>();

	private bool dirtyLayout;
}
