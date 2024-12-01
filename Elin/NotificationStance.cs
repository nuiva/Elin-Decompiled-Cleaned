public class NotificationStance : NotificationCondition
{
	public override bool Interactable => true;

	public override ItemNotice GetMold()
	{
		return WidgetStats.Instance.moldStance;
	}
}
