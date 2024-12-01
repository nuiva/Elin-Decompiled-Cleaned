using System.Collections.Generic;

public class WidgetStockTracker : Widget
{
	public static WidgetStockTracker Instance;

	public UIText text;

	public UIText textNum;

	private FastString sb = new FastString();

	private FastString lastSb = new FastString();

	private FastString sbNum = new FastString();

	private FastString lastSbNum = new FastString();

	public override void OnActivate()
	{
		Instance = this;
		Refresh();
	}

	public static void Refresh()
	{
		if ((bool)Instance)
		{
			Instance._Refresh();
		}
	}

	public void _Refresh()
	{
		sb.Clear();
		sbNum.Clear();
		HashSet<string> trackedCards = EMono.player.trackedCards;
		HashSet<string> trackedCategories = EMono.player.trackedCategories;
		if (trackedCards.Count == 0 && trackedCategories.Count == 0)
		{
			sb.Append("何もトラックしてない\n");
		}
		else
		{
			foreach (string item in trackedCards)
			{
				int num = EMono._map.Stocked.cardMap.TryGetValue(item)?.num ?? 0;
				sb.Append(EMono.sources.cards.map[item].GetName() + "\n");
				sbNum.Append(num + "\n");
			}
			foreach (string item2 in trackedCategories)
			{
				PropSetCategory propSetCategory = EMono._map.Stocked.categoryMap[item2];
				sb.Append(propSetCategory.source.GetText() + "\n");
				sbNum.Append(propSetCategory.sum + "\n");
			}
		}
		if (!sb.Equals(lastSb) || !sbNum.Equals(lastSbNum))
		{
			text.text = sb.ToString();
			textNum.text = sbNum.ToString();
			lastSb.Set(sb);
			lastSbNum.Set(sbNum);
			this.RebuildLayout();
		}
	}
}
