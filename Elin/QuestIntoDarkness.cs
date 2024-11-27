using System;

public class QuestIntoDarkness : QuestProgression
{
	public override string TitlePrefix
	{
		get
		{
			return "★";
		}
	}

	public override void OnStart()
	{
		EClass.game.quests.Add("demitas_spellwriter", null).startDate = EClass.world.date.GetRaw(0) + 1440;
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		return false;
	}
}
