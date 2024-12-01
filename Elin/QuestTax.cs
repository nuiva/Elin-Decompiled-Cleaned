public class QuestTax : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		return phase switch
		{
			0 => EClass.player.stats.taxBills > 0, 
			1 => EClass.player.stats.taxBillsPaid > 0, 
			_ => false, 
		};
	}

	public override void OnDropReward()
	{
		DropReward("mailpost");
	}

	public override void OnComplete()
	{
	}
}
