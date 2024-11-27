using System;
using System.Collections.Generic;
using System.Linq;

public class TraitBookSkill : TraitScroll
{
	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public int idEle
	{
		get
		{
			return this.owner.refVal;
		}
	}

	public override SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.idEle];
		}
	}

	public override int GetActDuration(Chara c)
	{
		return 5;
	}

	public virtual bool IsPlan
	{
		get
		{
			return false;
		}
	}

	public override void OnCreate(int lv)
	{
		IEnumerable<SourceElement.Row> ie = EClass.sources.elements.rows.Where(delegate(SourceElement.Row a)
		{
			if (a.chance == 0)
			{
				return false;
			}
			if (a.tag.Contains("hidden") || a.tag.Contains("unused"))
			{
				return false;
			}
			if (this.IsPlan)
			{
				if (a.category != "policy" && a.category != "tech")
				{
					return false;
				}
				if (a.cost.Length == 0 || (a.category == "tech" && a.cost[0] == 0))
				{
					return false;
				}
			}
			else if (a.category != "skill")
			{
				return false;
			}
			return true;
		});
		this.owner.refVal = ie.RandomItem<SourceElement.Row>().id;
	}

	public override void SetName(ref string s)
	{
		if (this.idEle == 0)
		{
			return;
		}
		string str = "";
		if ((EClass.sources.elements.map.TryGetValue(this.idEle, null) ?? EClass.sources.elements.map[0]).category == "policy")
		{
			str = " (" + "policy".lang() + ")";
		}
		s = "_of".lang((this.source.GetName() + str).Bracket(1), s, null, null, null);
		if (this.IsPlan && EClass.pc.homeBranch != null && EClass.pc.homeBranch.elements.HasBase(this.idEle))
		{
			s = s + " " + "alreadyLearned".lang();
		}
	}

	public override void OnRead(Chara c)
	{
		if (this.IsPlan && !c.IsPC)
		{
			return;
		}
		if (this.IsPlan && !EClass._zone.IsPCFaction)
		{
			this.owner.Say("skillbook_invalidZone", null, null);
			return;
		}
		if (this.IsPlan && EClass.Branch.elements.HasBase(this.idEle))
		{
			this.owner.Say("skillbook_knownSkill", c, this.source.GetName(), null);
			return;
		}
		this.owner.Say(this.IsPlan ? "skillbook_learnPlan" : "skillbook_learn", c, this.source.GetName(), null);
		if (this.IsPlan)
		{
			EClass.Branch.elements.Learn(this.idEle, 1);
			if (this.source.category == "policy" && !EClass.Branch.policies.HasPolicy(this.idEle))
			{
				EClass.Branch.policies.AddPolicy(this.idEle, true);
			}
			using (List<FactionBranch>.Enumerator enumerator = EClass.pc.faction.GetChildren().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FactionBranch factionBranch = enumerator.Current;
					factionBranch.ValidateUpgradePolicies();
				}
				goto IL_1A9;
			}
		}
		if (!c.elements.HasBase(this.idEle))
		{
			c.elements.Learn(this.idEle, 1);
		}
		else
		{
			c.elements.ModExp(this.idEle, this.owner.IsBlessed ? 1500 : (this.owner.IsCursed ? 500 : 1000), false);
		}
		IL_1A9:
		c.Say("spellbookCrumble", this.owner.Duplicate(1), null, null);
		this.owner.ModNum(-1, true);
	}

	public override int GetValue()
	{
		return this.owner.sourceCard.value;
	}

	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		if (!this.IsPlan)
		{
			n.Space(0, 1);
			foreach (Chara chara in EClass.pc.party.members)
			{
				bool flag = chara.elements.HasBase(this.idEle);
				n.AddText("_bullet".lang() + chara.Name + " " + (flag ? "alreadyLearned" : "notLearned").lang(), flag ? FontColor.Good : FontColor.Warning);
			}
		}
	}
}
