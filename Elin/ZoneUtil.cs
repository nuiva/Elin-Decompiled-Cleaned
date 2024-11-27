using System;

public class ZoneUtil
{
	public Map map
	{
		get
		{
			return this.zone.map;
		}
	}

	public bool AddBandits()
	{
		Point centerPos = this.map.bounds.GetCenterPos();
		for (int i = 0; i < 5; i++)
		{
			Chara t = CharaGen.CreateFromFilter("c_wilds", -1, -1);
			this.zone.AddCardSplinkle(t, centerPos, 5);
		}
		return true;
	}

	public bool AddMerchant(string id)
	{
		Point centerPos = this.map.bounds.GetCenterPos();
		Chara t = CharaGen.Create(id, -1);
		this.zone.AddCard(t, centerPos);
		return true;
	}

	public Zone zone;
}
