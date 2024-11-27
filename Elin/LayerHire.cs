using System;
using UnityEngine.Events;

public class LayerHire : ELayer
{
	public override void OnAfterInit()
	{
		this.Refresh();
	}

	public void Refresh()
	{
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<HireInfo, ButtonChara> callback = new UIList.Callback<HireInfo, ButtonChara>();
		callback.onClick = delegate(HireInfo a, ButtonChara b)
		{
		};
		callback.onInstantiate = delegate(HireInfo a, ButtonChara b)
		{
			Chara chara = a.chara;
			b.SetChara(chara, ButtonChara.Mode.Hire);
			CalcGold.Hire(chara);
			b.item.text2.text = (a.Days.ToString() ?? "");
			b.item.button1.onClick.AddListener(delegate()
			{
				a.isNew = false;
				this.Refresh();
				ELayer.ui.AddLayerDontCloseOthers<LayerChara>().SetChara(a.chara);
			});
			b.item.button2.onClick.AddListener(delegate()
			{
				a.isNew = false;
				a.chara.ShowDialog("_chara", "4-1", "").onKill.AddListener(new UnityAction(this.Refresh));
			});
			b.item.image1.SetActive(a.isNew);
		};
		callback.onRefresh = null;
		baseList.callbacks = callback;
		foreach (HireInfo hireInfo in ELayer.Home.listReserve)
		{
			if (!hireInfo.chara.IsHomeMember() && !hireInfo.chara.currentZone.IsPlayerFaction)
			{
				this.list.Add(hireInfo);
			}
		}
		this.list.Refresh(false);
		this.RebuildLayout(true);
	}

	public override void OnKill()
	{
		ELayer.Branch.ClearNewRecruits();
	}

	public UIList list;
}
