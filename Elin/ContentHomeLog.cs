using System;

public class ContentHomeLog : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		this.stat.Refresh(EClass.Branch.statistics);
		this.RefreshLog();
	}

	public void RefreshLog()
	{
		UIList uilist = this.listLog;
		BaseList baseList = uilist;
		UIList.Callback<MsgLog.Data, UIItem> callback = new UIList.Callback<MsgLog.Data, UIItem>();
		callback.onInstantiate = delegate(MsgLog.Data a, UIItem b)
		{
			b.text1.text = a.text.ToTitleCase(false);
			if (!a.col.IsEmpty())
			{
				b.text1.SetColor(a.col.ToEnum(true));
			}
			b.text2.text = string.Concat(new string[]
			{
				a.date.month.ToString(),
				"/",
				a.date.day.ToString(),
				" ",
				a.date.hour.ToString(),
				":",
				a.date.min.ToString()
			});
		};
		baseList.callbacks = callback;
		uilist.Clear();
		foreach (MsgLog.Data o in EClass.Branch.log.GetList(true))
		{
			uilist.Add(o);
		}
		uilist.Refresh(false);
		this.RebuildLayout(true);
	}

	public UIList listLog;

	public ItemStatistics stat;

	public ItemStatistics statLast;
}
