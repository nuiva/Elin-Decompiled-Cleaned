using System;
using UnityEngine;

public class TraitScrollMap : TraitScroll
{
	public string idSourceZone
	{
		get
		{
			return this.owner.GetStr(30, null);
		}
	}

	public SourceZone.Row sourceZone
	{
		get
		{
			return EClass.sources.zones.map[this.idSourceZone];
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override int GetActDuration(Chara c)
	{
		return 5;
	}

	public bool IsBlank
	{
		get
		{
			return this.idSourceZone.IsEmpty() || !EClass.sources.zones.map.ContainsKey(this.idSourceZone);
		}
	}

	public bool HasPrefix
	{
		get
		{
			return this.owner.GetInt(24, null) != 0;
		}
	}

	public override void SetName(ref string s)
	{
		if (!this.IsBlank)
		{
			s = "_of".lang(this.sourceZone.GetName(), s, null, null, null);
			if (this.HasPrefix)
			{
				s = EClass.sources.zoneAffixes.map[this.owner.GetInt(24, null)].GetName() + Lang.space + s;
			}
		}
		int @int = this.owner.GetInt(25, null);
		if (@int > 0)
		{
			s = s + " Lv " + (@int + 1).ToString();
		}
	}

	public override void OnRead(Chara c)
	{
		int @int = this.owner.GetInt(25, null);
		Zone zone = EClass.world.region.CreateRandomSite(EClass._zone, 8, this.idSourceZone, true, @int);
		if (zone == null)
		{
			Msg.Say("nothingHappens");
			return;
		}
		zone.isKnown = true;
		if (this.HasPrefix)
		{
			zone.idPrefix = this.owner.GetInt(24, null);
		}
		Msg.Say("discoverZone", zone.NameWithDangerLevel, null, null, null);
		this.owner.ModNum(-1, true);
		Debug.Log(string.Concat(new string[]
		{
			zone.Name,
			"/",
			zone.x.ToString(),
			"/",
			zone.y.ToString()
		}));
	}
}
