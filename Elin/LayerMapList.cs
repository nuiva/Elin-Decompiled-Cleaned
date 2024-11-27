using System;

public class LayerMapList : ELayer
{
	public override void OnInit()
	{
		this.RefreshList();
	}

	public void RefreshList()
	{
		this.list.sortMode = ELayer.player.pref.sortResearch;
		BaseList baseList = this.list;
		UIList.Callback<Zone, ItemGeneral> callback = new UIList.Callback<Zone, ItemGeneral>();
		callback.onInstantiate = delegate(Zone a, ItemGeneral b)
		{
			b.SetMainText(a.Name, null, true);
			b.Build();
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Zone zone in ELayer.game.spatials.Zones)
			{
				if (zone.isKnown)
				{
					this.list.Add(zone);
				}
			}
		};
		baseList.callbacks = callback;
		this.list.List(false);
	}

	public UIList list;
}
