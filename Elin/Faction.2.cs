using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Faction : EClass
{
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	public SourceFaction.Row source
	{
		get
		{
			SourceFaction.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = EClass.sources.factions.map[this.id]);
			}
			return result;
		}
	}

	public virtual string TextType
	{
		get
		{
			return "sub_faction".lang();
		}
	}

	public static Faction Create(SourceFaction.Row r)
	{
		Faction faction = ClassCache.Create<Faction>(r.type, "Elin");
		faction.id = r.id;
		faction.Init();
		return faction;
	}

	public void Init()
	{
		EClass.game.factions.AssignUID(this);
		this.relation.faction = this;
		this.relation.affinity = this.source.relation;
		this.name = this.source.GetText("name", false);
	}

	public void OnLoad()
	{
		this.relation.faction = this;
	}

	public float GetHappiness()
	{
		return 50f;
	}

	public Sprite GetSprite()
	{
		return ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/" + this.source.id);
	}

	public int CountTax()
	{
		return (int)((float)this.CountWealth() * 0.1f);
	}

	public List<FactionBranch> GetChildren()
	{
		List<FactionBranch> list = new List<FactionBranch>();
		foreach (Spatial spatial in EClass.game.spatials.map.Values)
		{
			if (spatial.mainFaction == this)
			{
				list.Add((spatial as Zone).branch);
			}
		}
		return list;
	}

	public int CountWealth()
	{
		if (EClass._zone.IsPCFaction)
		{
			EClass._zone.branch.resources.Refresh();
		}
		int num = 0;
		foreach (FactionBranch factionBranch in this.GetChildren())
		{
			num += factionBranch.Worth;
		}
		return num;
	}

	public int CountTerritories()
	{
		int num = 0;
		using (Dictionary<int, Spatial>.ValueCollection.Enumerator enumerator = EClass.game.spatials.map.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.mainFaction == this)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int CountMembers()
	{
		int num = 0;
		using (Dictionary<int, Chara>.ValueCollection.Enumerator enumerator = EClass.game.cards.globalCharas.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Chara.faction == this)
				{
					num++;
				}
			}
		}
		return num;
	}

	public Hostility GetHostility()
	{
		if (this == EClass.Home || this.relation.affinity >= 200)
		{
			return Hostility.Ally;
		}
		if (this.relation.affinity >= 100)
		{
			return Hostility.Friend;
		}
		if (this.relation.affinity <= -100)
		{
			return Hostility.Enemy;
		}
		return Hostility.Neutral;
	}

	public void ModRelation(int a)
	{
		this.relation.affinity += a;
	}

	public bool HasMember(string id, bool includeReserve = true)
	{
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.id == id && chara.IsHomeMember())
			{
				return true;
			}
		}
		if (includeReserve)
		{
			using (List<HireInfo>.Enumerator enumerator2 = this.listReserve.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.chara.id == id)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void AddContribution(int a)
	{
		if (a == 0 || this.relation.type != FactionRelation.RelationType.Member)
		{
			return;
		}
		this.relation.exp += a;
		Msg.Say("contribute", a.ToString() ?? "", this.Name, null, null);
	}

	public void AddReserve(Chara c)
	{
		if (c.memberType == FactionMemberType.Livestock)
		{
			c.SetInt(36, EClass.world.date.GetRaw(0) + 14400);
		}
		if (c.IsHomeMember())
		{
			c.homeBranch.RemoveMemeber(c);
		}
		if (c.currentZone != null)
		{
			c.currentZone.RemoveCard(c);
		}
		if (EClass.Branch.uidMaid == c.uid)
		{
			EClass.Branch.uidMaid = 0;
		}
		HireInfo item = new HireInfo
		{
			chara = c,
			isNew = true
		};
		this.listReserve.Add(item);
	}

	public void RemoveReserve(Chara c)
	{
		this.listReserve.ForeachReverse(delegate(HireInfo i)
		{
			if (i.chara == c || i.chara.uid == c.uid)
			{
				this.listReserve.Remove(i);
			}
		});
	}

	public void OnAdvanceDay()
	{
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			factionBranch.OnAdvanceDay();
		}
	}

	public void OnAdvanceMonth()
	{
		if (this.GetChildren().Count == 0)
		{
			return;
		}
		int num = 0;
		num += this.GetResidentTax();
		num += this.GetRankIncome();
		num += this.GetFactionSalary();
		if (num > 0)
		{
			Thing container_deposit = EClass.game.cards.container_deposit;
			if (EClass.pc.homeBranch.policies.IsActive(2711, -1) && (container_deposit.GetCurrency("money") > 0 || !container_deposit.things.IsFull(0)))
			{
				container_deposit.ModCurrency(num, "money");
				Msg.Say("bankIncome", Lang._currency(num, "money"), null, null, null);
			}
			else
			{
				Thing thing = ThingGen.Create("money", -1, -1).SetNum(num);
				Thing p = ThingGen.CreateParcel("parcel_salary", new Thing[]
				{
					thing
				});
				EClass.world.SendPackage(p);
			}
		}
		num = this.GetTotalTax(true);
		Thing thing2 = ThingGen.CreateBill(num, true);
		thing2.SetInt(35, EClass.player.extraTax);
		Msg.Say("getBill", Lang._currency(num, "money"), null, null, null);
		this.TryPayBill(thing2);
		Msg.Say("bills", EClass.player.taxBills.ToString() ?? "", null, null, null);
		if (EClass.player.taxBills >= 4)
		{
			EClass.player.ModKarma(-50);
		}
	}

	public void TryPayBill(Thing bill)
	{
		if (this.IsGlobalPolicyActive(2705) && EClass.game.cards.container_deposit.GetCurrency("money") > bill.c_bill)
		{
			InvOwnerDeliver.PayBill(bill, true);
			return;
		}
		EClass.world.SendPackage(bill);
	}

	public FactionBranch FindBranch(Chara c)
	{
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			if (factionBranch.members.Contains(c))
			{
				return factionBranch;
			}
		}
		return null;
	}

	public void AddGlobalPolicy(int id)
	{
		this.globalPolicies.Add(id);
	}

	public bool IsGlobalPolicyActive(int id)
	{
		bool result = false;
		using (List<FactionBranch>.Enumerator enumerator = EClass.pc.faction.GetChildren().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.policies.IsActive(id, -1))
				{
					result = true;
				}
			}
		}
		return result;
	}

	public void SetGlobalPolicyActive(int id, bool active)
	{
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			factionBranch.policies.SetActive(id, active);
		}
	}

	public int GetResidentTax()
	{
		int num = 0;
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			num += factionBranch.GetResidentTax();
		}
		if (num > 0)
		{
			Msg.Say("getResidentTax", Lang._currency(num, "money"), null, null, null);
		}
		return num;
	}

	public int GetRankIncome()
	{
		int num = 0;
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			num += EClass.game.spatials.ranks.GetIncome(factionBranch.owner);
		}
		if (num > 0)
		{
			Msg.Say("getRankIncome", Lang._currency(num, "money"), null, null, null);
		}
		return num;
	}

	public int GetFactionSalary()
	{
		int num = 0;
		foreach (Faction faction in EClass.game.factions.dictAll.Values)
		{
			num += faction.relation.GetSalary();
		}
		if (num > 0)
		{
			Msg.Say("getFactionSalary", Lang._currency(num, "money"), null, null, null);
		}
		return num;
	}

	public int GetTotalTax(bool evasion)
	{
		return this.GetBaseTax(evasion) + this.GetFameTax(evasion) + EClass.player.extraTax;
	}

	public int GetBaseTax(bool evasion)
	{
		int a = EClass.world.date.year - EClass.game.Prologue.year;
		int v = 500 + Mathf.Min(a, 10) * 500;
		return this.EvadeTax(v, evasion);
	}

	public int GetFameTax(bool evasion)
	{
		int v = EClass.curve(EClass.player.fame * 2, 10000, 2000, 80);
		return this.EvadeTax(v, evasion);
	}

	public int EvadeTax(int v, bool evasion)
	{
		if (!evasion)
		{
			return v;
		}
		int num = 0;
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			num += factionBranch.Evalue(2119);
		}
		return (int)((float)(v * 100) / (100f + Mathf.Sqrt((float)(num * 5))));
	}

	public void SetTaxTooltip(UINote n)
	{
		n.AddHeader("tax", null);
		n.AddTopic("tax_base", Lang._currency(this.GetBaseTax(true), true, 14) + " (" + Lang._currency(this.GetBaseTax(false), false, 14) + ")");
		n.AddTopic("tax_fame", Lang._currency(this.GetFameTax(true), true, 14) + " (" + Lang._currency(this.GetFameTax(false), false, 14) + ")");
		n.AddTopic("tax_extra", Lang._currency(EClass.player.extraTax, true, 14));
	}

	public int CountTaxFreeLand()
	{
		int num = 0;
		using (List<FactionBranch>.Enumerator enumerator = EClass.pc.faction.GetChildren().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.policies.IsActive(2514, -1))
				{
					num++;
				}
			}
		}
		return num;
	}

	[JsonProperty]
	public FactionRelation relation = new FactionRelation();

	[JsonProperty]
	public int maxReserve = 3;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string uid;

	[JsonProperty]
	public string name;

	[JsonProperty]
	public List<HireInfo> listReserve = new List<HireInfo>();

	[JsonProperty]
	public ElementContainerZone elements = new ElementContainerZone();

	[JsonProperty]
	public HashSet<int> globalPolicies = new HashSet<int>();

	public ElementContainerFaction charaElements = new ElementContainerFaction();

	public SourceFaction.Row _source;
}
