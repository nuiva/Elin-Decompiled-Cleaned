using System;

public class NotificationStance : NotificationCondition
{
	public override ItemNotice GetMold()
	{
		return WidgetStats.Instance.moldStance;
	}

	public override bool Interactable
	{
		get
		{
			return true;
		}
	}
}
