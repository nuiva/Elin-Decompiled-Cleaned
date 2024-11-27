using System;

public class QuestPuppy : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		return this.phase == 1 && EClass._map.FindChara("poppy") != null;
	}

	public override void OnDropReward()
	{
		base.DropReward("coolerbox");
	}
}
