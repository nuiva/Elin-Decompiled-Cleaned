using UnityEngine;

public class ButtonResourceTrack : UIButton
{
	public PropSet prop;

	public PropSetCategory cat;

	public UIResourceTrack track => UIResourceTrack.Instance;

	public void SetProp(string id)
	{
		Debug.Log(id);
		prop = EClass._map.Stocked.cardMap.GetOrCreate(id);
		CardRow cardRow = EClass.sources.cards.map[id];
		tooltip.text = cardRow.GetName();
		tooltip.enable = true;
		cardRow.SetImage(icon);
		onRightClick = delegate
		{
			EClass.player.trackedCards.Remove(id);
			track._Refresh();
			SE.Trash();
		};
		Refresh();
	}

	public void SetCat(string id)
	{
		cat = EClass._map.Stocked.categoryMap[id];
		tooltip.text = cat.source.GetName();
		tooltip.enable = true;
		onRightClick = delegate
		{
			EClass.player.trackedCategories.Remove(id);
			track._Refresh();
			SE.Trash();
		};
		Refresh();
	}

	public void Refresh()
	{
		if (cat != null)
		{
			subText.SetText(cat.sum.ToString() ?? "");
			return;
		}
		int num = ((prop != null) ? prop.num : 0);
		subText.SetText(num.ToString() ?? "");
	}
}
