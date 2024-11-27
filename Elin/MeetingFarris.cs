using System;

public class MeetingFarris : Meeting
{
	public override string IdChara
	{
		get
		{
			return "farris";
		}
	}

	public override bool IsGlobalChara
	{
		get
		{
			return true;
		}
	}

	public override void PlayDrama()
	{
		LayerDrama.ActivateMain("farris", "1-1", this.chara, null, "");
	}
}
