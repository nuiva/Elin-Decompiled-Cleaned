using System;

public class NotificationStatsMana : NotificationStats
{
	public override bool Visible
	{
		get
		{
			return true;
		}
	}

	public override int idSprite
	{
		get
		{
			return 4;
		}
	}
}
