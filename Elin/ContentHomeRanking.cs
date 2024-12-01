using System.Collections.Generic;
using UnityEngine;

public class ContentHomeRanking : EContent
{
	public UIDynamicList listRanking;

	public Color[] colors;

	public override void OnSwitchContent(int idTab)
	{
		Refresh();
	}

	public void Refresh()
	{
		List<RankedZone> ranks = EClass.game.spatials.ranks.GetList();
		UIDynamicList list = listRanking;
		list.Clear();
		list.callbacks = new UIList.Callback<RankedZone, UIItem>
		{
			onClick = delegate
			{
			},
			onRedraw = delegate(RankedZone a, UIItem b, int i)
			{
				b.text1.text = ((a.z.visitCount > 0) ? a.Name : "?????");
				b.text2.text = $"{a.Value:#,0}";
				b.text3.text = a.rank.ToString() ?? "";
				b.text4.text = a.GetFactionName();
				b.image1.sprite = a.GetSprite() ?? b.image1.sprite;
				b.image1.SetNativeSize();
				b.text3.color = colors[(a.rank > 3) ? ((a.rank <= 10) ? 1 : ((a.rank <= 100) ? 2 : 3)) : 0];
				b.image2.SetActive(a.z.IsPCFaction);
			},
			onList = delegate
			{
				foreach (RankedZone item in ranks)
				{
					list.Add(item);
				}
			},
			onRefresh = null
		};
		list.List();
		int num = -1;
		foreach (RankedZone item2 in ranks)
		{
			if (item2.z == EClass._zone)
			{
				num = list.objects.IndexOf(item2);
			}
		}
		if (num != -1)
		{
			list.dsv.scrollByItemIndex(num);
			list.Refresh();
		}
		this.RebuildLayout(recursive: true);
	}
}
