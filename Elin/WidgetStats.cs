using System;
using System.Collections.Generic;

public class WidgetStats : BaseWidgetNotice
{
	public static void RefreshAll()
	{
		if (!WidgetStats.Instance)
		{
			return;
		}
		WidgetStats.Instance._RefreshAll();
	}

	public override void _OnActivate()
	{
		WidgetStats.Instance = this;
		NotificationStats notificationStats = new NotificationStats();
		notificationStats.stats = (() => EMono.pc.hunger);
		base.Add(notificationStats, null);
		NotificationStats notificationStats2 = new NotificationStats();
		notificationStats2.stats = (() => EMono.pc.burden);
		base.Add(notificationStats2, null);
		NotificationStats notificationStats3 = new NotificationStats();
		notificationStats3.stats = (() => EMono.pc.depression);
		base.Add(notificationStats3, null);
		NotificationStats notificationStats4 = new NotificationStats();
		notificationStats4.stats = (() => EMono.pc.bladder);
		base.Add(notificationStats4, null);
		NotificationStats notificationStats5 = new NotificationStats();
		notificationStats5.stats = (() => EMono.pc.hygiene);
		base.Add(notificationStats5, null);
		NotificationStats notificationStats6 = new NotificationStats();
		notificationStats6.stats = (() => EMono.pc.sleepiness);
		base.Add(notificationStats6, null);
		NotificationStats notificationStats7 = new NotificationStats();
		notificationStats7.stats = (() => EMono.pc.stamina);
		base.Add(notificationStats7, null);
		base.Add(new NotificationExceedParty(), null);
	}

	public override void OnRefresh()
	{
		this.conditions.ForeachReverse(delegate(NotificationCondition a)
		{
			if (a.ShouldRemove())
			{
				this.conditions.Remove(a);
				base.Remove(a);
			}
		});
		foreach (Condition condition in EMono.pc.conditions)
		{
			if (condition.ShowInWidget)
			{
				bool flag = true;
				using (List<NotificationCondition>.Enumerator enumerator2 = this.conditions.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.condition == condition)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					NotificationCondition notificationCondition = condition.CreateNotification() as NotificationCondition;
					base.Add(notificationCondition, null);
					this.conditions.Add(notificationCondition);
				}
			}
		}
	}

	public static WidgetStats Instance;

	public List<NotificationCondition> conditions = new List<NotificationCondition>();

	public ItemNotice moldBuff;

	public ItemNotice moldStance;
}
