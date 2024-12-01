using System.Collections.Generic;

public class NoticeManager
{
	public class Notice
	{
		public string Text => "tempNotice".lang();
	}

	public List<Notice> list = new List<Notice>();

	public void Refresh()
	{
		list.Clear();
		list.Add(new Notice());
		list.Add(new Notice());
		list.Add(new Notice());
		if (list.Count == 0)
		{
			list.Add(new Notice());
		}
	}
}
