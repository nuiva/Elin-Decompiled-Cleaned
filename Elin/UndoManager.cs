using System;
using System.Collections.Generic;

public class UndoManager
{
	public UndoManager.Item lastItem
	{
		get
		{
			return this.items.LastItem<UndoManager.Item>();
		}
	}

	public void Validate()
	{
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			if (this.items[i].Count() == 0)
			{
				this.items.RemoveAt(i);
			}
		}
	}

	public void NewItem()
	{
		this.items.Add(new UndoManager.Item());
		if (this.items.Count > 10)
		{
			this.items.RemoveAt(0);
		}
	}

	public void Add(Task t)
	{
		this.lastItem.list.Add(t);
	}

	public string GetText()
	{
		string text = "tUndo".lang() + Environment.NewLine;
		if (this.items.Count == 0)
		{
			text += "tUndoNone".lang();
		}
		else
		{
			text += "tUndoNote".lang(this.lastItem.Count().ToString() ?? "", this.lastItem.name ?? "", null, null, null);
		}
		return text;
	}

	public void WriteNote(UINote n)
	{
		this.Validate();
		n.Clear();
		n.Space(10, 1);
		n.AddText("NoteText_topic", "tUndo".lang(), FontColor.DontChange);
		if (this.items.Count == 0)
		{
			n.AddText("tUndoNone".lang(), FontColor.DontChange);
		}
		else
		{
			n.AddText("tUndoNote".lang(this.lastItem.Count().ToString() ?? "", this.lastItem.name ?? "", null, null, null), FontColor.DontChange);
		}
		n.Build();
	}

	public void Perform()
	{
		this.Validate();
		if (this.items.Count == 0)
		{
			SE.Beep();
			return;
		}
		foreach (Task task in this.lastItem.list)
		{
			task.Destroy();
		}
		this.items.Remove(this.lastItem);
		SE.Play("trash");
	}

	public List<UndoManager.Item> items = new List<UndoManager.Item>();

	public class Item
	{
		public string name
		{
			get
			{
				if (this.list.Count <= 0)
				{
					return "none".lang();
				}
				return this.list[0].Name;
			}
		}

		public int Count()
		{
			int num = 0;
			using (List<Task>.Enumerator enumerator = this.list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.isDestroyed)
					{
						num++;
					}
				}
			}
			return num;
		}

		public List<Task> list = new List<Task>();
	}
}
