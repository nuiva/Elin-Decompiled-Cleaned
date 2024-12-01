public class QuestSharedContainer : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (phase == 0)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled && thing.IsSharedContainer)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnComplete()
	{
		EClass.game.quests.globalList.Add(Quest.Create("tax").SetClient(EClass.game.cards.globalCharas.Find("ashland"), assignQuest: false));
		EClass.game.quests.globalList.Add(Quest.Create("introInspector").SetClient(EClass.game.cards.globalCharas.Find("ashland"), assignQuest: false));
	}
}
