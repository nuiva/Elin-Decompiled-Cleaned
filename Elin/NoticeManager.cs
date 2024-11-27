using System;
using System.Collections.Generic;

public class NoticeManager
{
	public void Refresh()
	{
		this.list.Clear();
		this.list.Add(new NoticeManager.Notice());
		this.list.Add(new NoticeManager.Notice());
		this.list.Add(new NoticeManager.Notice());
		if (this.list.Count == 0)
		{
			this.list.Add(new NoticeManager.Notice());
		}
	}

	public List<NoticeManager.Notice> list = new List<NoticeManager.Notice>();

	public class Notice
	{
		public string Text
		{
			get
			{
				return "tempNotice".lang();
			}
		}
	}
}
