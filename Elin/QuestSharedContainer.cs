using System;

public class QuestSharedContainer : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (this.phase == 0)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled && thing.IsSharedContainer)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public override void OnComplete()
	{
		EClass.game.quests.globalList.Add(Quest.Create("tax", null, null).SetClient(EClass.game.cards.globalCharas.Find("ashland"), false));
		EClass.game.quests.globalList.Add(Quest.Create("introInspector", null, null).SetClient(EClass.game.cards.globalCharas.Find("ashland"), false));
	}
}
