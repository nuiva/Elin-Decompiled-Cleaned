public class LayerHire : ELayer
{
	public UIList list;

	public override void OnAfterInit()
	{
		Refresh();
	}

	public void Refresh()
	{
		list.Clear();
		list.callbacks = new UIList.Callback<HireInfo, ButtonChara>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(HireInfo a, ButtonChara b)
			{
				Chara chara = a.chara;
				b.SetChara(chara, ButtonChara.Mode.Hire);
				CalcGold.Hire(chara);
				b.item.text2.text = a.Days.ToString() ?? "";
				b.item.button1.onClick.AddListener(delegate
				{
					a.isNew = false;
					Refresh();
					ELayer.ui.AddLayerDontCloseOthers<LayerChara>().SetChara(a.chara);
				});
				b.item.button2.onClick.AddListener(delegate
				{
					a.isNew = false;
					a.chara.ShowDialog("_chara", "4-1").onKill.AddListener(Refresh);
				});
				b.item.image1.SetActive(a.isNew);
			},
			onRefresh = null
		};
		foreach (HireInfo item in ELayer.Home.listReserve)
		{
			if (!item.chara.IsHomeMember() && !item.chara.currentZone.IsPlayerFaction)
			{
				list.Add(item);
			}
		}
		list.Refresh();
		this.RebuildLayout(recursive: true);
	}

	public override void OnKill()
	{
		ELayer.Branch.ClearNewRecruits();
	}
}
