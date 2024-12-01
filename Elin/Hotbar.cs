using System.Collections.Generic;
using Newtonsoft.Json;

public class Hotbar : EClass
{
	public enum Type
	{
		Default,
		Main,
		ZoomMenu
	}

	public class Page : EClass
	{
		[JsonProperty]
		public List<HotItem> items = new List<HotItem>();

		[JsonProperty]
		public int selected = -1;

		public HotItem SelectedItem
		{
			get
			{
				if (selected != -1)
				{
					return items[selected];
				}
				return null;
			}
		}

		public void SetItem(Hotbar h, HotItem item, int index)
		{
			if (index == -1)
			{
				for (int i = 0; i < items.Count; i++)
				{
					if (items[i] == null)
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
			if (items[index] != null)
			{
				items[index].button = null;
			}
			items[index] = item;
		}

		public HotItem GetItem(int index)
		{
			if (index >= items.Count)
			{
				return null;
			}
			return items[index];
		}
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
	public List<Page> pages = new List<Page>();

	public bool dirty;

	public WidgetHotbar actor;

	public HotbarManager manager => EClass.player.hotbars;

	public HotItem SelectedItem => pages[currentPage].SelectedItem;

	public HotItem DefaultItem => null;

	public bool IsLocked => id == 3;

	public bool IsUserHotbar
	{
		get
		{
			if (id != 5)
			{
				return id == 6;
			}
			return true;
		}
	}

	public bool ShowFunctionKey => id == 5;

	public Page CurrentPage => pages[(currentPage >= 0) ? currentPage : 0];

	public void SetSlotNum(int a)
	{
		itemsPerPage = a;
		foreach (Page page in pages)
		{
			ValidatePage(page);
		}
	}

	public void AddPage()
	{
		Page page = new Page();
		pages.Add(page);
		ValidatePage(page);
	}

	public void ValidatePage(Page page, int num = -1)
	{
		if (num != -1 && itemsPerPage < num)
		{
			itemsPerPage = num;
		}
		if (page.items.Count < itemsPerPage)
		{
			int num2 = itemsPerPage - page.items.Count;
			for (int i = 0; i < num2; i++)
			{
				page.items.Add(DefaultItem);
			}
		}
	}

	public void SetPage(int pageIndex)
	{
		currentPage = pageIndex;
	}

	public void Remove(HotItem item)
	{
		for (int i = 0; i < pages.Count; i++)
		{
			for (int j = 0; j < pages[i].items.Count; j++)
			{
				if (pages[i].items[j] == item)
				{
					SetItem(null, j, i, refreshActor: true);
				}
			}
		}
	}

	public HotItem GetItem(int index, int pageIndex = -1)
	{
		return pages[(pageIndex == -1) ? currentPage : pageIndex].GetItem(index);
	}

	public HotItem SetItem(HotItem item, int index = -1, int pageIndex = -1, bool refreshActor = false)
	{
		if (item == null)
		{
			item = DefaultItem;
		}
		if (pageIndex == -1)
		{
			pageIndex = currentPage;
		}
		if (pageIndex >= pages.Count)
		{
			AddPage();
			pageIndex = pages.Count - 1;
		}
		Page page = pages[pageIndex];
		ValidatePage(page, index + 1);
		page.SetItem(this, item, index);
		item?.OnAddedToBar();
		if (refreshActor && (bool)actor)
		{
			actor.RebuildPage();
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
			pageIndex = currentPage;
		}
		HotItem selectedItem = pages[pageIndex].SelectedItem;
		pages[pageIndex].selected = -1;
		selectedItem?.OnUnselect();
		EClass.player.SetCurrentHotItem(null);
	}

	public HotItem GetSelectedItem()
	{
		int selected = pages[currentPage].selected;
		if (selected == -1)
		{
			return null;
		}
		return pages[currentPage].items[selected];
	}

	public int GetNextSelectableIndex(int pageIndex = -1)
	{
		if (pageIndex == -1)
		{
			pageIndex = currentPage;
		}
		Page page = pages[pageIndex];
		int num = page.selected + 1;
		if (num >= itemsPerPage)
		{
			num = -1;
		}
		ValidatePage(page);
		return num;
	}

	public int GetPrevSelectableIndex(int pageIndex = -1)
	{
		if (pageIndex == -1)
		{
			pageIndex = currentPage;
		}
		Page page = pages[pageIndex];
		int num = page.selected - 1;
		if (num < -1)
		{
			num = itemsPerPage - 1;
		}
		ValidatePage(page);
		return num;
	}
}
