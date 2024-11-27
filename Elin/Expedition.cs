using System;
using Newtonsoft.Json;

public class Expedition : EClass
{
	public Chara chara
	{
		get
		{
			return RefChara.Get(this.uidChara);
		}
	}

	public int MinHour
	{
		get
		{
			return (int)((float)this.hours * 0.8f);
		}
	}

	public int MaxHour
	{
		get
		{
			return (int)((float)this.hours * 1.2f);
		}
	}

	public string strType
	{
		get
		{
			return ("ex" + this.type.ToString()).lang();
		}
	}

	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
	}

	public void OnCreate()
	{
		HomeResourceManager resources = this.branch.resources;
		switch (this.type)
		{
		case ExpeditionType.Search:
			this.hours = 24;
			return;
		case ExpeditionType.Training:
			this.hours = 120;
			return;
		case ExpeditionType.Explore:
			this.hours = 10;
			return;
		case ExpeditionType.Hunt:
			this.hours = 10;
			return;
		default:
			this.hours = 24;
			return;
		}
	}

	public void Start()
	{
		this.hours = Rand.Range(this.MinHour, this.MaxHour);
		this.costs.Pay();
		WidgetPopText.Say("expeditionStart".lang(this.chara.Name, this.strType, null, null, null), FontColor.Default, null);
		EClass.Branch.Log("bExpeditionStart", this.chara, this.strType, null, null);
		if (this.chara.IsInActiveZone)
		{
			Msg.Say("bExpeditionStart", this.chara, this.strType, null, null);
		}
		this.chara.MoveZone("somewhere");
	}

	public void OnAdvanceHour()
	{
		this.hours--;
		if (this.hours <= 0)
		{
			this.End();
		}
	}

	public void End()
	{
		this.branch.expeditions.dict.Remove(this.uidChara);
		this.chara.MoveZone(this.chara.homeZone, ZoneTransition.EnterState.Auto);
		WidgetPopText.Say("expeditionEnd".lang(this.chara.Name, this.strType, null, null, null), FontColor.Default, null);
		this.branch.Log("bExpeditionEnd", this.chara, this.strType, null, null);
		if (this.chara.IsInActiveZone)
		{
			Msg.Say("bExpeditionEnd", this.chara, this.strType, null, null);
		}
		ExpeditionType expeditionType = this.type;
		if (expeditionType != ExpeditionType.Search)
		{
			if (expeditionType != ExpeditionType.Explore)
			{
				return;
			}
			this.chara.GetWorkSummary().progress = 100;
		}
		else
		{
			foreach (Zone zone in EClass.world.region.ListZonesInRadius(this.branch.owner, 10))
			{
				if (!zone.isKnown)
				{
					zone.isKnown = true;
					WidgetPopText.Say("discoverZone".lang(zone.Name, null, null, null, null), FontColor.Great, null);
				}
			}
			Zone zone2 = EClass.world.region.CreateRandomSite(this.chara.homeZone, 8, null, true, 0);
			if (zone2 != null)
			{
				zone2.isKnown = true;
				WidgetPopText.Say("discoverZone".lang(zone2.Name, null, null, null, null), FontColor.Great, null);
				return;
			}
		}
	}

	public static Expedition Create(Chara c, ExpeditionType type)
	{
		Expedition expedition = new Expedition();
		expedition.uidChara = c.uid;
		expedition.type = type;
		expedition.SetOwner(c.homeBranch);
		expedition.OnCreate();
		return expedition;
	}

	[JsonProperty]
	public int hours;

	[JsonProperty]
	public int uidChara;

	[JsonProperty]
	public ExpeditionType type;

	public HomeResource.CostList costs = new HomeResource.CostList();

	public FactionBranch branch;
}
