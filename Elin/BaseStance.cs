using System;

public class BaseStance : Condition
{
	public override bool CanManualRemove
	{
		get
		{
			return true;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override BaseNotification CreateNotification()
	{
		return new NotificationStance
		{
			condition = this
		};
	}

	public override void Tick()
	{
	}
}
