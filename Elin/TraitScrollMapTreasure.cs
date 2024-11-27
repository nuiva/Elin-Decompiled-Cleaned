using System;

public class TraitScrollMapTreasure : TraitItem
{
	public override string LangUse
	{
		get
		{
			return "actRead";
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.refVal = EClass.rnd(30000) + 1;
		this.owner.LV = lv;
	}

	public override bool OnUse(Chara c)
	{
		if (this.owner.refVal == 0)
		{
			this.owner.refVal = EClass.rnd(30000) + 1;
		}
		if (this.GetDest(false) == null || EClass._map.IsIndoor)
		{
			Msg.Say("nothingHappens");
			return false;
		}
		Rand.SetSeed(this.owner.refVal);
		if (this.owner.blessedState <= BlessedState.Cursed && EClass.rnd(2) == 0)
		{
			Msg.Say("mapCrumble", this.owner, null, null, null);
			this.owner.Destroy();
			return false;
		}
		Rand.SetSeed(-1);
		LayerTreasureMap layerTreasureMap = EClass.ui.layerFloat.ToggleLayer<LayerTreasureMap>(null);
		if (layerTreasureMap != null)
		{
			layerTreasureMap.SetMap(this);
		}
		return false;
	}

	public Point GetDest(bool fix = false)
	{
		Point point = new Point();
		int num = this.owner.GetInt(104, null);
		if (num == 0)
		{
			Rand.SetSeed(this.owner.refVal);
			for (int i = 0; i < 10000; i++)
			{
				point.x = EClass.scene.elomap.minX + EClass.rnd(200);
				point.z = EClass.scene.elomap.minY + EClass.rnd(200);
				if (EClass.scene.elomap.CanBuildSite(point.x, point.z, 1, ElomapSiteType.Treasure))
				{
					Rand.SetSeed(-1);
					num = (point.x + 500) * 1000 + (point.z + 500);
					this.owner.SetInt(104, num);
					break;
				}
			}
			Rand.SetSeed(-1);
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
