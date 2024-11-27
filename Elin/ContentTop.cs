using System;

public class ContentTop : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		this.textHomeName.text = Lang.Parse("journalTitle", EClass.Home.name, null, null, null, null);
		EClass.player.notices.Refresh();
		BaseList baseList = this.list2;
		UIList.Callback<Schedule.Item, UIItem> callback = new UIList.Callback<Schedule.Item, UIItem>();
		callback.onInstantiate = delegate(Schedule.Item a, UIItem b)
		{
			b.text1.text = a.Name;
			b.text2.text = ((a.date == null) ? "????" : a.date.GetText(Date.TextFormat.Schedule));
		};
		baseList.callbacks = callback;
		this.list2.Clear();
		foreach (Schedule.Item o in EClass.world.schedule.list)
		{
			this.list2.Add(o);
		}
		this.list2.Refresh(false);
	}

	public UIText textHomeName;

	public UIList list1;

	public UIList list2;
}
