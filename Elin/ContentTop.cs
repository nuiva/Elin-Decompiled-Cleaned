public class ContentTop : EContent
{
	public UIText textHomeName;

	public UIList list1;

	public UIList list2;

	public override void OnSwitchContent(int idTab)
	{
		textHomeName.text = Lang.Parse("journalTitle", EClass.Home.name);
		EClass.player.notices.Refresh();
		list2.callbacks = new UIList.Callback<Schedule.Item, UIItem>
		{
			onInstantiate = delegate(Schedule.Item a, UIItem b)
			{
				b.text1.text = a.Name;
				b.text2.text = ((a.date == null) ? "????" : a.date.GetText(Date.TextFormat.Schedule));
			}
		};
		list2.Clear();
		foreach (Schedule.Item item in EClass.world.schedule.list)
		{
			list2.Add(item);
		}
		list2.Refresh();
	}
}
