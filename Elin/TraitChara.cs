using System;
using UnityEngine;

public class TraitChara : Trait
{
	public new Chara owner
	{
		get
		{
			return this.owner.Chara;
		}
	}

	public virtual AI_Idle.Behaviour IdleBehaviour
	{
		get
		{
			return AI_Idle.Behaviour.Default;
		}
	}

	public virtual bool CanAutoRevive
	{
		get
		{
			return this.owner.isImported;
		}
	}

	public virtual bool IsCitizen
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsUnique
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanRevive
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanGuide
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanIdentify
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanPicklock
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanInvest
	{
		get
		{
			return false;
		}
	}

	public virtual string IDTrainer
	{
		get
		{
			return "";
		}
	}

	public virtual bool CanJoinParty
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanJoinPartyResident
	{
		get
		{
			return !this.owner.GetBool(18) || EClass.pc.CHA >= this.owner.GetBestAttribute();
		}
	}

	public virtual bool CanSellStolenGoods
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBeBanished
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanInvite
	{
		get
		{
			return !this.owner.source.multisize;
		}
	}

	public virtual bool IsCountAsResident
	{
		get
		{
			return !this.owner.IsUnique;
		}
	}

	public virtual bool CanInvestTown
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanSellPlan
	{
		get
		{
			return this.owner.IsMaid && EClass.BranchOrHomeBranch != null;
		}
	}

	public virtual bool CanHeal
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanWhore
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanServeFood
	{
		get
		{
			return false;
		}
	}

	public virtual bool HaveNews
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanBout
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseGlobalGoal
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowAdvRank
	{
		get
		{
			return this.owner.IsPC;
		}
	}

	public virtual bool UseRandomAbility
	{
		get
		{
			return false;
		}
	}

	public virtual TraitChara.Adv_Type AdvType
	{
		get
		{
			return TraitChara.Adv_Type.None;
		}
	}

	public virtual bool EnableTone
	{
		get
		{
			return !this.IsUnique;
		}
	}

	public virtual bool CanBeTamed
	{
		get
		{
			return !this.IsUnique && this.owner.rarity < Rarity.Legendary && !this.owner.IsMultisize && !this.owner.IsGlobal && !this.owner.IsPCFaction && EClass._zone.Boss != this.owner;
		}
	}

	public virtual bool CanBePushed
	{
		get
		{
			return !this.owner.source.multisize;
		}
	}

	public virtual bool CanGiveRandomQuest
	{
		get
		{
			return (this.owner.IsPCFaction || (this.IsCitizen && !this.IsUnique)) && this.owner.quest == null;
		}
	}

	public virtual bool UseRandomAlias
	{
		get
		{
			return false;
		}
	}

	public override string IDInvStyle
	{
		get
		{
			return "backpack";
		}
	}

	public virtual string IDRumor
	{
		get
		{
			return "";
		}
	}

	public virtual bool IdleAct()
	{
		return false;
	}

	public virtual string IdAmbience
	{
		get
		{
			return null;
		}
	}

	public virtual bool CanFish
	{
		get
		{
			return this.owner.job.id == "tourist";
		}
	}

	public virtual string GetDramaText()
	{
		if (this.CanInvest)
		{
			return "dramaText_shop".lang((this.owner.c_invest + 1).ToString() ?? "", null, null, null, null);
		}
		return "";
	}

	public override int GetValue()
	{
		return this.owner.LV * 500 + 3000;
	}

	public int GetAdvRank()
	{
		int min = 0;
		string id = this.owner.id;
		if (id == "adv_gaki" || id == "adv_kiria" || id == "adv_ivory")
		{
			min = 4;
		}
		return Mathf.Clamp(this.owner.LV / 10, min, 6);
	}

	public string GetAdvRankText()
	{
		int advRank = this.GetAdvRank();
		return TraitChara.ListRank[advRank];
	}

	public static string[] ListRank = new string[]
	{
		"E",
		"D",
		"C",
		"B",
		"A",
		"S",
		"S"
	};

	public enum Adv_Type
	{
		None,
		Adv,
		Adv_Fairy,
		Adv_Backer
	}
}
