using System;
using System.Collections.Generic;

public class UndoManager
{
	public class Item
	{
		public List<Task> list = new List<Task>();

		public string name
		{
			get
			{
				if (list.Count <= 0)
				{
					return "none".lang();
				}
				return list[0].Name;
			}
		}

		public int Count()
		{
			int num = 0;
			foreach (Task item in list)
			{
				if (!item.isDestroyed)
				{
					num++;
				}
			}
			return num;
		}
	}

	public List<Item> items = new List<Item>();

	public Item lastItem => items.LastItem();

	public void Validate()
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			if (items[num].Count() == 0)
			{
				items.RemoveAt(num);
			}
		}
	}

	public void NewItem()
	{
		items.Add(new Item());
		if (items.Count > 10)
		{
			items.RemoveAt(0);
		}
	}

	public void Add(Task t)
	{
		lastItem.list.Add(t);
	}

	public string GetText()
	{
		string text = "";
		text = "tUndo".lang() + Environment.NewLine;
		if (items.Count == 0)
		{
			return text + "tUndoNone".lang();
		}
		return text + "tUndoNote".lang(lastItem.Count().ToString() ?? "", lastItem.name ?? "");
	}

	public void WriteNote(UINote n)
	{
		Validate();
		n.Clear();
		n.Space(10);
		n.AddText("NoteText_topic", "tUndo".lang());
		if (items.Count == 0)
		{
			n.AddText("tUndoNone".lang());
		}
		else
		{
			n.AddText("tUndoNote".lang(lastItem.Count().ToString() ?? "", lastItem.name ?? ""));
		}
		n.Build();
	}

	public void Perform()
	{
		Validate();
		if (items.Count == 0)
		{
			SE.Beep();
			return;
		}
		foreach (Task item in lastItem.list)
		{
			item.Destroy();
		}
		items.Remove(lastItem);
		SE.Play("trash");
	}
}
