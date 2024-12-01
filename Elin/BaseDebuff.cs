public class BaseDebuff : Condition
{
	public override bool WillOverride => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override BaseNotification CreateNotification()
	{
		return new NotificationBuff
		{
			condition = this
		};
	}
}
