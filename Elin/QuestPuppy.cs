public class QuestPuppy : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (phase == 1)
		{
			return EClass._map.FindChara("poppy") != null;
		}
		return false;
	}

	public override void OnDropReward()
	{
		DropReward("coolerbox");
	}
}
