using System;
using System.Collections.Generic;

[Serializable]
public class CardFilter : EClass
{
	public override string ToString()
	{
		return this.id;
	}

	protected virtual bool _ShouldPass(CardRow source)
	{
		return true;
	}

	public virtual bool ContainsTag(CardRow source, string str)
	{
		return source.tag.Contains(str);
	}

	public void Init()
	{
		this.BuildList(this.tags, this.strTag);
		this.BuildList(this.filters, this.strFilter);
		if (!this.filterCategory.IsEmpty())
		{
			foreach (string text in this.filterCategory)
			{
				bool flag = text.StartsWith('-');
				SourceCategory.Row row = EClass.sources.categories.map[flag ? text.Remove(0, 1) : text];
				this.categories.Add(new CardFilter.FilterCategory
				{
					exclude = flag,
					row = row
				});
				if (!flag)
				{
					this.categoriesInclude.Add(row);
				}
			}
		}
		this.isInitialied = true;
	}

	public bool Pass(CardRow source)
	{
		if (!this.isInitialied)
		{
			this.Init();
		}
		if (source.chance == 0 || source.isOrigin)
		{
			return false;
		}
		for (int i = 0; i < this.tags.Count; i++)
		{
			CardFilter.FilterItem filterItem = this.tags[i];
			if (filterItem.exclude)
			{
				if (this.ContainsTag(source, filterItem.name))
				{
					return false;
				}
			}
			else if (!this.ContainsTag(source, filterItem.name))
			{
				return false;
			}
		}
		for (int j = 0; j < this.filters.Count; j++)
		{
			CardFilter.FilterItem filterItem2 = this.filters[j];
			if (filterItem2.exclude)
			{
				if (source.filter.Contains(filterItem2.name))
				{
					return false;
				}
			}
			else if (!source.filter.Contains(filterItem2.name))
			{
				return false;
			}
		}
		if ((this.isChara && !source.isChara) || (!this.isChara && source.isChara))
		{
			return false;
		}
		if (this.categories.Count > 0)
		{
			bool flag = false;
			foreach (CardFilter.FilterCategory filterCategory in this.categories)
			{
				if ((!flag || filterCategory.exclude) && EClass.sources.categories.map[source.category].IsChildOf(filterCategory.row))
				{
					if (filterCategory.exclude)
					{
						return false;
					}
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return this._ShouldPass(source);
	}

	public void BuildList(List<CardFilter.FilterItem> list, string[] strs)
	{
		if (strs == null)
		{
			return;
		}
		foreach (string text in strs)
		{
			list.Add(CardFilter.GetFilterItem(text));
		}
	}

	public static CardFilter.FilterItem GetFilterItem(string text)
	{
		bool flag = text.StartsWith("-");
		return new CardFilter.FilterItem
		{
			exclude = flag,
			name = (flag ? text.Substring(1) : text)
		};
	}

	public string id;

	public string[] strTag;

	public string[] strFilter;

	public string[] filterCategory;

	public string[] idCard;

	public List<CardFilter.FilterItem> tags = new List<CardFilter.FilterItem>();

	public List<CardFilter.FilterItem> filters = new List<CardFilter.FilterItem>();

	public List<CardFilter.FilterCategory> categories = new List<CardFilter.FilterCategory>();

	public List<SourceCategory.Row> categoriesInclude = new List<SourceCategory.Row>();

	private bool isInitialied;

	public bool isChara;

	public class FilterItem
	{
		public bool exclude;

		public string name;
	}

	public class FilterCategory
	{
		public bool exclude;

		public SourceCategory.Row row;
	}

	public enum FilterMode
	{
		None,
		Exclude,
		Match,
		Equipped
	}
}
