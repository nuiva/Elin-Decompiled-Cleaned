using System;

public class NotificationGlobal : BaseNotification
{
	public virtual WidgetNotice widget
	{
		get
		{
			return WidgetNotice.Instance;
		}
	}
}
