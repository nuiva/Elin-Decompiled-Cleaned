using System;

public class MeetingMerchant : Meeting
{
	public override string IdChara
	{
		get
		{
			return "merchant";
		}
	}

	public override void PlayDrama()
	{
		this.chara.ShowDialog();
	}
}
