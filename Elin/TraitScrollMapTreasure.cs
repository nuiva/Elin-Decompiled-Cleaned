public class TraitScrollMapTreasure : TraitItem
{
	public override string LangUse => "actRead";

	public override bool CanStack => false;

	public override void OnCreate(int lv)
	{
		owner.refVal = EClass.rnd(30000) + 1;
		owner.LV = lv;
	}

	public override bool OnUse(Chara c)
	{
		if (owner.refVal == 0)
		{
			owner.refVal = EClass.rnd(30000) + 1;
		}
		if (GetDest() == null || EClass._map.IsIndoor)
		{
			Msg.Say("nothingHappens");
			return false;
		}
		Rand.SetSeed(owner.refVal);
		if (owner.blessedState <= BlessedState.Cursed && EClass.rnd(2) == 0)
		{
			Msg.Say("mapCrumble", owner);
			owner.Destroy();
			return false;
		}
		Rand.SetSeed();
		EClass.ui.layerFloat.ToggleLayer<LayerTreasureMap>()?.SetMap(this);
		return false;
	}

	public Point GetDest(bool fix = false)
	{
		Point point = new Point();
		int num = owner.GetInt(104);
		if (num == 0)
		{
			Rand.SetSeed(owner.refVal);
			for (int i = 0; i < 10000; i++)
			{
				point.x = EClass.scene.elomap.minX + EClass.rnd(200);
				point.z = EClass.scene.elomap.minY + EClass.rnd(200);
				if (EClass.scene.elomap.CanBuildSite(point.x, point.z, 1, ElomapSiteType.Treasure))
				{
					Rand.SetSeed();
					num = (point.x + 500) * 1000 + (point.z + 500);
					owner.SetInt(104, num);
					break;
				}
			}
			Rand.SetSeed();
			if (num == 0)
			{
				return null;
			}
		}
		point = new Point(num / 1000 - 500, num % 1000 - 500);
		if (fix)
		{
			point.x -= EClass.scene.elomap.minX;
			point.z -= EClass.scene.elomap.minY;
		}
		return point;
	}
}
