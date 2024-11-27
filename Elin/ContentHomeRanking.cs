using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentHomeRanking : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		this.Refresh();
	}

	public void Refresh()
	{
		List<RankedZone> ranks = EClass.game.spatials.ranks.GetList();
		UIDynamicList list = this.listRanking;
		list.Clear();
		BaseList list2 = list;
		UIList.Callback<RankedZone, UIItem> callback = new UIList.Callback<RankedZone, UIItem>();
		callback.onClick = delegate(RankedZone a, UIItem b)
		{
		};
		callback.onRedraw = delegate(RankedZone a, UIItem b, int i)
		{
			b.text1.text = ((a.z.visitCount > 0) ? a.Name : "?????");
			b.text2.text = string.Format("{0:#,0}", a.Value);
			b.text3.text = (a.rank.ToString() ?? "");
			b.text4.text = a.GetFactionName();
			b.image1.sprite = (a.GetSprite() ?? b.image1.sprite);
			b.image1.SetNativeSize();
			b.text3.color = this.colors[(a.rank <= 3) ? 0 : ((a.rank <= 10) ? 1 : ((a.rank <= 100) ? 2 : 3))];
			b.image2.SetActive(a.z.IsPCFaction);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (RankedZone o in ranks)
			{
				list.Add(o);
			}
		};
		callback.onRefresh = null;
		list2.callbacks = callback;
		list.List();
		int num = -1;
		foreach (RankedZone rankedZone in ranks)
		{
			if (rankedZone.z == EClass._zone)
			{
				num = list.objects.IndexOf(rankedZone);
			}
		}
		if (num != -1)
		{
			list.dsv.scrollByItemIndex(num);
			list.Refresh();
		}
		this.RebuildLayout(true);
	}

	public UIDynamicList listRanking;

	public Color[] colors;
}
