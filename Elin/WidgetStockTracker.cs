using System;
using System.Collections.Generic;

public class WidgetStockTracker : Widget
{
	public override void OnActivate()
	{
		WidgetStockTracker.Instance = this;
		WidgetStockTracker.Refresh();
	}

	public static void Refresh()
	{
		if (WidgetStockTracker.Instance)
		{
			WidgetStockTracker.Instance._Refresh();
		}
	}

	public void _Refresh()
	{
		this.sb.Clear();
		this.sbNum.Clear();
		HashSet<string> trackedCards = EMono.player.trackedCards;
		HashSet<string> trackedCategories = EMono.player.trackedCategories;
		if (trackedCards.Count == 0 && trackedCategories.Count == 0)
		{
			this.sb.Append("何もトラックしてない\n");
		}
		else
		{
			foreach (string key in trackedCards)
			{
				PropSet propSet = EMono._map.Stocked.cardMap.TryGetValue(key, null);
				int num = (propSet != null) ? propSet.num : 0;
				this.sb.Append(EMono.sources.cards.map[key].GetName() + "\n");
				this.sbNum.Append(num.ToString() + "\n");
			}
			foreach (string key2 in trackedCategories)
			{
				PropSetCategory propSetCategory = EMono._map.Stocked.categoryMap[key2];
				this.sb.Append(propSetCategory.source.GetText("name", false) + "\n");
				this.sbNum.Append(propSetCategory.sum.ToString() + "\n");
			}
		}
		if (this.sb.Equals(this.lastSb) && this.sbNum.Equals(this.lastSbNum))
		{
			return;
		}
		this.text.text = this.sb.ToString();
		this.textNum.text = this.sbNum.ToString();
		this.lastSb.Set(this.sb);
		this.lastSbNum.Set(this.sbNum);
		this.RebuildLayout(false);
	}

	public static WidgetStockTracker Instance;

	public UIText text;

	public UIText textNum;

	private FastString sb = new FastString(32);

	private FastString lastSb = new FastString(32);

	private FastString sbNum = new FastString(32);

	private FastString lastSbNum = new FastString(32);
}
