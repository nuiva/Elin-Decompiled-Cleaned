using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentKeyItem : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		this.list.sortMode = UIList.SortMode.ByValue;
		BaseList baseList = this.list;
		UIList.Callback<SourceKeyItem.Row, ItemGeneral> callback = new UIList.Callback<SourceKeyItem.Row, ItemGeneral>();
		callback.onInstantiate = delegate(SourceKeyItem.Row a, ItemGeneral b)
		{
			b.SetSound(null);
			b.button1.mainText.SetText(a.GetName());
			b.SetSubText(EClass.player.CountKeyItem(a.id).ToString() ?? "", 260, FontColor.Default, TextAnchor.MiddleRight);
		};
		callback.onClick = delegate(SourceKeyItem.Row a, ItemGeneral b)
		{
			this.SelectItem(a);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (int num in EClass.player.keyItems.Keys)
			{
				if (EClass.player.CountKeyItem(num) > 0)
				{
					this.list.Add(EClass.sources.keyItems.map[num]);
				}
			}
		};
		baseList.callbacks = callback;
		this.list.List(false);
		this.SelectItem(EClass.sources.keyItems.rows[0]);
	}

	public void SelectItem(SourceKeyItem.Row q)
	{
		this.textTitle.SetText(q.GetName());
		string detail = q.GetDetail();
		this.textDetail.SetText(detail);
		Sprite sprite = Resources.Load<Sprite>("Media/Graphics/Image/KeyItem/" + q.alias);
		if (sprite)
		{
			this.imageItem.sprite = sprite;
		}
		this.imageItem.SetNativeSize();
		this.imageItem.SetActive(sprite);
		this.RebuildLayout(true);
	}

	public UIList list;

	public UIText textClient;

	public UIText textTitle;

	public UIText textDetail;

	public Image imageItem;
}
