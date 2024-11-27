using System;
using UnityEngine;

public class ButtonResourceTrack : UIButton
{
	public UIResourceTrack track
	{
		get
		{
			return UIResourceTrack.Instance;
		}
	}

	public void SetProp(string id)
	{
		Debug.Log(id);
		this.prop = EClass._map.Stocked.cardMap.GetOrCreate(id, null);
		CardRow cardRow = EClass.sources.cards.map[id];
		this.tooltip.text = cardRow.GetName();
		this.tooltip.enable = true;
		cardRow.SetImage(this.icon, null, 0, true, 0, 0);
		this.onRightClick = delegate()
		{
			EClass.player.trackedCards.Remove(id);
			this.track._Refresh();
			SE.Trash();
		};
		this.Refresh();
	}

	public void SetCat(string id)
	{
		this.cat = EClass._map.Stocked.categoryMap[id];
		this.tooltip.text = this.cat.source.GetName();
		this.tooltip.enable = true;
		this.onRightClick = delegate()
		{
			EClass.player.trackedCategories.Remove(id);
			this.track._Refresh();
			SE.Trash();
		};
		this.Refresh();
	}

	public void Refresh()
	{
		if (this.cat != null)
		{
			this.subText.SetText(this.cat.sum.ToString() ?? "");
			return;
		}
		int num = (this.prop != null) ? this.prop.num : 0;
		this.subText.SetText(num.ToString() ?? "");
	}

	public PropSet prop;

	public PropSetCategory cat;
}
