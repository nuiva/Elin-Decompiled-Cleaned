using System;

public class ContentStatistics : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		NumLogManager nums = EClass.player.nums;
		BaseList baseList = this.list1;
		UIList.Callback<NumLog, ItemNumLog> callback = new UIList.Callback<NumLog, ItemNumLog>();
		callback.onInstantiate = delegate(NumLog a, ItemNumLog b)
		{
			b.SetLog(a);
		};
		baseList.callbacks = callback;
		BaseList baseList2 = this.list2;
		UIList.Callback<NumLog, ItemNumLog> callback2 = new UIList.Callback<NumLog, ItemNumLog>();
		callback2.onInstantiate = delegate(NumLog a, ItemNumLog b)
		{
			b.SetLog(a);
		};
		baseList2.callbacks = callback2;
		foreach (NumLog o in nums.listCategory)
		{
			this.list1.Add(o);
		}
		foreach (NumLog o2 in nums.listImportant)
		{
			this.list2.Add(o2);
		}
		this.list1.Refresh(false);
		this.list2.Refresh(false);
		this.RebuildLayout(true);
	}

	public UIList list1;

	public UIList list2;
}
