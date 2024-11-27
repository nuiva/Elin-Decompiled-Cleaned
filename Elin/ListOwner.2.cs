using System;

public class ListOwner : EClass
{
	public ListOwner Main
	{
		get
		{
			if (!this.main)
			{
				return this.other;
			}
			return this;
		}
	}

	public virtual string IdTitle
	{
		get
		{
			return base.GetType().Name;
		}
	}

	public virtual string IdHeaderRow
	{
		get
		{
			return null;
		}
	}

	public virtual string TextTab
	{
		get
		{
			return this.textTab.lang();
		}
	}

	public virtual string TextHeader
	{
		get
		{
			if (!this.textHeader.IsEmpty())
			{
				return this.textHeader.lang();
			}
			return "";
		}
	}

	public virtual void List()
	{
	}

	public virtual void OnCreate()
	{
	}

	public virtual void OnSwitchContent()
	{
		this.List();
		this.OnRefreshMenu();
		if (Lang.GetList(this.IdTitle) != null)
		{
			this.window.SetTitles(this.IdTitle, this.IdHeaderRow);
		}
		if (!this.textHeader.IsEmpty())
		{
			this.window.SetCaption(this.TextHeader);
		}
	}

	public virtual void OnRefreshMenu()
	{
		this.window.menuLeft.Clear();
		this.window.menuRight.Clear();
	}

	public void MoveToOther(object c)
	{
		this.list.RemoveDynamic(c);
		this.other.List();
		this.list.RebuildLayoutTo(this.layer);
		SE.Resource();
	}

	public Layer layer;

	public Window window;

	public UIMultiList multi;

	public ListOwner other;

	public UIList list;

	public WindowMenu menu;

	public string textTab;

	public string textHeader;

	public bool main;

	public int index;
}
