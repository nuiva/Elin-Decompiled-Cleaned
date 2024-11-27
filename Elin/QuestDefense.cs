using System;

public class QuestDefense : QuestProgression
{
	public int numRequired
	{
		get
		{
			return 100;
		}
	}

	public int numHunted
	{
		get
		{
			return EClass.player.stats.kills;
		}
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		switch (this.phase)
		{
		case 0:
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.isRestrained && chara.id == "punk" && chara.ExistsOnMap && chara.pos.IsInBounds)
				{
					return false;
				}
			}
			return true;
		case 1:
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.isRestrained && chara2.id == "boar" && chara2.ExistsOnMap && chara2.pos.IsInBounds)
				{
					return false;
				}
			}
			return true;
		case 2:
			return this.numHunted >= this.numRequired;
		default:
			return false;
		}
		bool result;
		return result;
	}

	public override void OnChangePhase(int a)
	{
		if (this.phase == 2)
		{
			EClass.game.quests.globalList.Add(Quest.Create("puppy", null, null).SetClient(EClass.game.cards.globalCharas.Find("fiama"), false));
		}
	}

	public override string GetTextProgress()
	{
		if (this.phase != 2)
		{
			return "";
		}
		return "progressHunt".lang(this.numHunted.ToString() ?? "", this.numRequired.ToString() ?? "", null, null, null);
	}

	public override void OnDropReward()
	{
		base.DropReward(ThingGen.Create("plat", -1, -1).SetNum(10));
	}
}
