using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceTrack : EMono
{
	public static UIResourceTrack Instance;

	public LayoutGroup layout;

	public ButtonResourceTrack mold;

	public UIButton buttonAdd;

	public Dictionary<string, ButtonResourceTrack> dictProp = new Dictionary<string, ButtonResourceTrack>();

	public Dictionary<string, ButtonResourceTrack> dictCat = new Dictionary<string, ButtonResourceTrack>();

	private List<string> listRemove = new List<string>();

	private bool dirtyLayout;

	public static void Refresh()
	{
		if ((bool)Instance)
		{
			Instance._Refresh();
		}
	}

	public void OnActivate()
	{
		Instance = this;
		buttonAdd.SetOnClick(delegate
		{
			EMono.ui.AddLayer<LayerResource>();
		});
		if (!mold)
		{
			mold = layout.CreateMold<ButtonResourceTrack>();
		}
		_Refresh();
	}

	public void OnMoveZone()
	{
		dictProp.Clear();
		dictCat.Clear();
		layout.DestroyChildren();
		_Refresh();
	}

	public void _Refresh()
	{
		dirtyLayout = false;
		HashSet<string> trackedCards = EMono.player.trackedCards;
		HashSet<string> trackedCategories = EMono.player.trackedCategories;
		foreach (string item in trackedCards)
		{
			if (dictProp.ContainsKey(item))
			{
				dictProp[item].Refresh();
				continue;
			}
			ButtonResourceTrack buttonResourceTrack = Util.Instantiate(mold, layout);
			buttonResourceTrack.SetProp(item);
			dictProp.Add(item, buttonResourceTrack);
			dirtyLayout = true;
		}
		foreach (string item2 in trackedCategories)
		{
			if (dictCat.ContainsKey(item2))
			{
				dictCat[item2].Refresh();
				continue;
			}
			ButtonResourceTrack buttonResourceTrack2 = Util.Instantiate(mold, layout);
			buttonResourceTrack2.SetCat(item2);
			dictCat.Add(item2, buttonResourceTrack2);
			dirtyLayout = true;
		}
		listRemove.Clear();
		foreach (string key in dictProp.Keys)
		{
			if (!trackedCards.Contains(key))
			{
				listRemove.Add(key);
			}
		}
		foreach (string item3 in listRemove)
		{
			Object.DestroyImmediate(dictProp[item3].gameObject);
			dictProp.Remove(item3);
			dirtyLayout = true;
		}
		listRemove.Clear();
		foreach (string key2 in dictCat.Keys)
		{
			if (!trackedCategories.Contains(key2))
			{
				listRemove.Add(key2);
			}
		}
		foreach (string item4 in listRemove)
		{
			Object.DestroyImmediate(dictCat[item4].gameObject);
			dictCat.Remove(item4);
			dirtyLayout = true;
		}
		if (dirtyLayout)
		{
			buttonAdd.transform.SetAsLastSibling();
			layout.RebuildLayout(recursive: true);
		}
	}
}
