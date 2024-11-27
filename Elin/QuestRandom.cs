using System;

public class QuestRandom : Quest
{
	public override bool CanAutoAdvance
	{
		get
		{
			return false;
		}
	}

	public override bool IsRandomQuest
	{
		get
		{
			return true;
		}
	}

	public override int RangeDeadLine
	{
		get
		{
			return 12;
		}
	}

	public override int KarmaOnFail
	{
		get
		{
			return -5;
		}
	}

	public override int FameOnComplete
	{
		get
		{
			return 4 + this.difficulty * 2;
		}
	}

	public override string RefDrama1
	{
		get
		{
			return Lang._currency(this.rewardMoney, true, 0);
		}
	}

	public override void OnDropReward()
	{
		int num = this.bonusMoney * (55 + this.difficulty * 15) / 100;
		int num2 = this.rewardMoney + num;
		if (num2 > 0)
		{
			if (num > 0)
			{
				Msg.Say("reward_bonus", num.ToString() ?? "", null, null, null);
			}
			base.DropReward(ThingGen.CreateCurrency(num2, "money"));
		}
		Zone zone = EClass._zone.GetTopZone();
		if ((!zone.IsTown || zone.IsPCFaction) && base.ClientZone != null)
		{
			Zone topZone = base.ClientZone.GetTopZone();
			if (topZone.IsTown && !topZone.IsPCFaction)
			{
				zone = topZone;
			}
		}
		Rand.SetSeed(this.uid);
		string id = Util.EnumToList<Quest.SubReward>().RandomItem<Quest.SubReward>().ToString();
		if (EClass.rnd(5) == 0)
		{
			id = "gacha_coin_silver";
		}
		if (EClass.rnd(15) == 0)
		{
			id = "gacha_coin_gold";
		}
		Thing thing = ThingGen.Create(id, -1, -1);
		if (thing.id == "ticket_furniture")
		{
			TraitTicketFurniture.SetZone(zone, thing);
		}
		base.DropReward(thing);
		thing = ThingGen.Create("plat", -1, -1).SetNum(this.GetRewardPlat(num2));
		base.DropReward(thing);
		Rand.SetSeed(-1);
		if (zone.IsTown || zone.IsPCFaction)
		{
			zone.GetTopZone().ModInfluence(1);
		}
		if (this.FameOnComplete > 0)
		{
			EClass.player.ModFame(EClass.rndHalf(this.FameOnComplete));
		}
	}
}
