public class ZoneUtil
{
	public Zone zone;

	public Map map => zone.map;

	public bool AddBandits()
	{
		Point centerPos = map.bounds.GetCenterPos();
		for (int i = 0; i < 5; i++)
		{
			Chara t = CharaGen.CreateFromFilter("c_wilds");
			zone.AddCardSplinkle(t, centerPos, 5);
		}
		return true;
	}

	public bool AddMerchant(string id)
	{
		Point centerPos = map.bounds.GetCenterPos();
		Chara t = CharaGen.Create(id);
		zone.AddCard(t, centerPos);
		return true;
	}
}
