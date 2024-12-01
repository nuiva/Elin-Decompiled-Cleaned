public class QuestDefense : QuestProgression
{
	public int numRequired => 100;

	public int numHunted => EClass.player.stats.kills;

	public override bool CanUpdateOnTalk(Chara c)
	{
		switch (phase)
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
			return numHunted >= numRequired;
		default:
			return false;
		}
	}

	public override void OnChangePhase(int a)
	{
		if (phase == 2)
		{
			EClass.game.quests.globalList.Add(Quest.Create("puppy").SetClient(EClass.game.cards.globalCharas.Find("fiama"), assignQuest: false));
		}
	}

	public override string GetTextProgress()
	{
		if (phase != 2)
		{
			return "";
		}
		return "progressHunt".lang(numHunted.ToString() ?? "", numRequired.ToString() ?? "");
	}

	public override void OnDropReward()
	{
		DropReward(ThingGen.Create("plat").SetNum(10));
	}
}
