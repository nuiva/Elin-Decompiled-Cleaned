using System;
using Newtonsoft.Json;
using UnityEngine;

public class FactionRelation : EClass
{
	public int ExpToNext
	{
		get
		{
			return 100 + this.rank * this.rank * 100;
		}
	}

	public int MaxRank
	{
		get
		{
			return 5;
		}
	}

	public int GetSalary()
	{
		if (this.type != FactionRelation.RelationType.Member)
		{
			return 0;
		}
		return this.rank * 100;
	}

	public string TextTitle
	{
		get
		{
			return Lang.GetList("guild_title")[Mathf.Min(this.rank / 2, 5)];
		}
	}

	public string GetTextRelation()
	{
		return ("faction_" + this.type.ToString()).lang();
	}

	public void SetTextHostility(UIText text)
	{
		if (this.IsMember() || this.affinity > 100)
		{
			text.SetText("reFriend".lang(), FontColor.Good);
			return;
		}
		if (this.affinity < -100)
		{
			text.SetText("reEnemy".lang(), FontColor.Bad);
			return;
		}
		text.SetText("reNeutral".lang(), FontColor.Passive);
	}

	public bool IsMember()
	{
		return this.type == FactionRelation.RelationType.Member || this.type == FactionRelation.RelationType.Owner;
	}

	public void Promote()
	{
		this.exp -= this.ExpToNext;
		this.rank++;
		Msg.Say("faction_promote");
		SE.Play("questComplete");
	}

	[JsonProperty]
	public int affinity;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int rank;

	[JsonProperty]
	public FactionRelation.RelationType type;

	public Faction faction;

	public enum RelationType
	{
		Default,
		Owner,
		Member
	}
}
