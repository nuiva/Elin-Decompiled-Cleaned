using System;

public class QuestTax : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		int phase = this.phase;
		if (phase != 0)
		{
			return phase == 1 && EClass.player.stats.taxBillsPaid > 0;
		}
		return EClass.player.stats.taxBills > 0;
	}

	public override void OnDropReward()
	{
		base.DropReward("mailpost");
	}

	public override void OnComplete()
	{
	}
}
