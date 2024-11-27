using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Hotbar : EClass
{
	public HotbarManager manager
	{
		get
		{
			return EClass.player.hotbars;
		}
	}

	public HotItem SelectedItem
	{
		get
		{
			return this.pages[this.currentPage].SelectedItem;
		}
	}

	public HotItem DefaultItem
	{
		get
		{
			return null;
		}
	}

	public bool IsLocked
	{
		get
		{
			return this.id == 3;
		}
	}

	public bool IsUserHotbar
	{
		get
		{
			return this.id == 5 || this.id == 6;
		}
	}

	public bool ShowFunctionKey
	{
		get
		{
			return this.id == 5;
		}
	}

	public Hotbar.Page CurrentPage
	{
		get
		{
			return this.pages[(this.currentPage >= 0) ? this.currentPage : 0];
		}
	}

	public void SetSlotNum(int a)
	{
		this.itemsPerPage = a;
		foreach (Hotbar.Page page in this.pages)
		{
			this.ValidatePage(page, -1);
		}
	}

	public void AddPage()
	{
		Hotbar.Page page = new Hotbar.Page();
		this.pages.Add(page);
		this.ValidatePage(page, -1);
	}

	public void ValidatePage(Hotbar.Page page, int num = -1)
	{
		if (num != -1 && this.itemsPerPage < num)
		{
			this.itemsPerPage = num;
		}
		if (page.items.Count < this.itemsPerPage)
		{
			int num2 = this.itemsPerPage - page.items.Count;
			for (int i = 0; i < num2; i++)
			{
				page.items.Add(this.DefaultItem);
			}
		}
	}

	public void SetPage(int pageIndex)
	{
		this.currentPage = pageIndex;
	}

	public void Remove(HotItem item)
	{
		for (int i = 0; i < this.pages.Count; i++)
		{
			for (int j = 0; j < this.pages[i].items.Count; j++)
			{
				if (this.pages[i].items[j] == item)
				{
					this.SetItem(null, j, i, true);
				}
			}
		}
	}

	public HotItem GetItem(int index, int pageIndex = -1)
	{
		return this.pages[(pageIndex == -1) ? this.currentPage : pageIndex].GetItem(index);
	}

	public HotItem SetItem(HotItem item, int index = -1, int pageIndex = -1, bool refreshActor = false)
	{
		if (item == null)
		{
			item = this.DefaultItem;
		}
		if (pageIndex == -1)
		{
			pageIndex = this.currentPage;
		}
		if (pageIndex >= this.pages.Count)
		{
			this.AddPage();
			pageIndex = this.pages.Count - 1;
		}
		Hotbar.Page page = this.pages[pageIndex];
		this.ValidatePage(page, index + 1);
		page.SetItem(this, item, index);
		if (item != null)
		{
			item.OnAddedToBar();
		}
		if (refreshActor && this.actor)
		{
			this.actor.RebuildPage(-1);
		}
		return item;
	}

	public void Select(HotItem item)
	{
		SE.SelectHotitem();
		EClass.player.SetCurrentHotItem(item);
	}

	public void ToggleDisable(HotItem item)
	{
		item.disabled = !item.disabled;
		if (item.disabled)
		{
			item.OnUnselect();
		}
		SE.SelectHotitem();
		EClass.player.SetCurrentHotItem(item.disabled ? null : item);
	}

	public void Unselect(int pageIndex = -1)
	{
		if (pageIndex == -1)
		{
			pageIndex = this.currentPage;
		}
		HotItem selectedItem = this.pages[pageIndex].SelectedItem;
		this.pages[pageIndex].selected = -1;
		if (selectedItem != null)
		{
			selectedItem.OnUnselect();
		}
		EClass.player.SetCurrentHotItem(null);
	}

	public HotItem GetSelectedItem()
	{
		int selected = this.pages[this.currentPage].selected;
		if (selected == -1)
		{
			return null;
		}
		return this.pages[this.currentPage].items[selected];
	}

	public int GetNextSelectableIndex(int pageIndex = -1)
	{
		if (pageIndex == -1)
		{
			pageIndex = this.currentPage;
		}
		Hotbar.Page page = this.pages[pageIndex];
		int num = page.selected + 1;
		if (num >= this.itemsPerPage)
		{
			num = -1;
		}
		this.ValidatePage(page, -1);
		return num;
	}

	public int GetPrevSelectableIndex(int pageIndex = -1)
	{
		if (pageIndex == -1)
		{
			pageIndex = this.currentPage;
		}
		Hotbar.Page page = this.pages[pageIndex];
		int num = page.selected - 1;
		if (num < -1)
		{
			num = this.itemsPerPage - 1;
		}
		this.ValidatePage(page, -1);
		return num;
	}

	public const int IDMainMenu = 2;

	public const int IDBuild = 3;

	public const int IDUser2 = 5;

	public const int IDUser3 = 6;

	public const int IDSpeed = 7;

	[JsonProperty]
	public int currentPage;

	[JsonProperty]
	public int itemsPerPage = 6;

	[JsonProperty]
	public int id;

	[JsonProperty]
	public List<Hotbar.Page> pages = new List<Hotbar.Page>();

	public bool dirty;

	public WidgetHotbar actor;

	public enum Type
	{
		Default,
		Main,
		ZoomMenu
	}

	public class Page : EClass
	{
		public HotItem SelectedItem
		{
			get
			{
				if (this.selected != -1)
				{
					return this.items[this.selected];
				}
				return null;
			}
		}

		public void SetItem(Hotbar h, HotItem item, int index)
		{
			if (index == -1)
			{
				for (int i = 0; i < this.items.Count; i++)
				{
					if (this.items[i] == null)
					{
						index = i;
						break;
					}
				}
				if (index == -1)
				{
					return;
				}
			}
			if (this.items[index] != null)
			{
				this.items[index].button = null;
			}
			this.items[index] = item;
		}

		public HotItem GetItem(int index)
		{
			if (index >= this.items.Count)
			{
				return null;
			}
			return this.items[index];
		}

		[JsonProperty]
		public List<HotItem> items = new List<HotItem>();

		[JsonProperty]
		public int selected = -1;
	}
}
