public class LayerMapList : ELayer
{
	public UIList list;

	public override void OnInit()
	{
		RefreshList();
	}

	public void RefreshList()
	{
		list.sortMode = ELayer.player.pref.sortResearch;
		list.callbacks = new UIList.Callback<Zone, ItemGeneral>
		{
			onInstantiate = delegate(Zone a, ItemGeneral b)
			{
				b.SetMainText(a.Name);
				b.Build();
			},
			onList = delegate
			{
				foreach (Zone zone in ELayer.game.spatials.Zones)
				{
					if (zone.isKnown)
					{
						list.Add(zone);
					}
				}
			}
		};
		list.List();
	}
}
