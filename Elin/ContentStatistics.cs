public class ContentStatistics : EContent
{
	public UIList list1;

	public UIList list2;

	public override void OnSwitchContent(int idTab)
	{
		NumLogManager nums = EClass.player.nums;
		list1.callbacks = new UIList.Callback<NumLog, ItemNumLog>
		{
			onInstantiate = delegate(NumLog a, ItemNumLog b)
			{
				b.SetLog(a);
			}
		};
		list2.callbacks = new UIList.Callback<NumLog, ItemNumLog>
		{
			onInstantiate = delegate(NumLog a, ItemNumLog b)
			{
				b.SetLog(a);
			}
		};
		foreach (NumLog item in nums.listCategory)
		{
			list1.Add(item);
		}
		foreach (NumLog item2 in nums.listImportant)
		{
			list2.Add(item2);
		}
		list1.Refresh();
		list2.Refresh();
		this.RebuildLayout(recursive: true);
	}
}
