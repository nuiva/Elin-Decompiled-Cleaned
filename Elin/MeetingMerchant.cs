public class MeetingMerchant : Meeting
{
	public override string IdChara => "merchant";

	public override void PlayDrama()
	{
		chara.ShowDialog();
	}
}
