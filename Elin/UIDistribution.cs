using System;

public class UIDistribution : EMono
{
	private void OnEnable()
	{
		if (this.needRefresh)
		{
			this.needRefresh = false;
			this.Refresh();
		}
	}

	public void SetContainer(Card t, Window.SaveData d)
	{
		this.container = t;
		this.data = d;
		this.needRefresh = true;
		this.SetActive(false);
	}

	public void Refresh()
	{
		UIDistribution.<>c__DisplayClass6_0 CS$<>8__locals1 = new UIDistribution.<>c__DisplayClass6_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.cats = this.data.cats;
		BaseList baseList = this.list;
		UIList.Callback<SourceCategory.Row, ButtonCategory> callback = new UIList.Callback<SourceCategory.Row, ButtonCategory>();
		callback.onClick = delegate(SourceCategory.Row a, ButtonCategory b)
		{
			if (a.children.Count > 0)
			{
				b.buttonFold.onClick.Invoke();
			}
		};
		callback.onInstantiate = delegate(SourceCategory.Row a, ButtonCategory b)
		{
			b.uid = a.uid;
			b.mainText.text = a.GetName().ToTitleCase(false);
			bool flag = false;
			foreach (int uid in CS$<>8__locals1.cats)
			{
				if (a.Contatin(uid))
				{
					flag = true;
					break;
				}
			}
			b.SetFold(a.children.Count > 0, !flag, delegate(UIList l)
			{
				foreach (SourceCategory.Row o in a.children)
				{
					l.Add(o);
				}
			});
			b.buttonToggle.icon.SetActive(CS$<>8__locals1.cats.Contains(a.uid));
			b.buttonToggle.SetOnClick(delegate
			{
				bool flag2 = !CS$<>8__locals1.cats.Contains(a.uid);
				b.buttonToggle.icon.SetActive(flag2);
				if (flag2)
				{
					CS$<>8__locals1.cats.Add(a.uid);
				}
				else
				{
					CS$<>8__locals1.cats.Remove(a.uid);
				}
				CS$<>8__locals1.<Refresh>g__SetAll|0(a.uid, flag2);
				SE.Click();
			});
		};
		callback.onRefresh = null;
		baseList.callbacks = callback;
		foreach (SourceCategory.Row row in EMono.sources.categories.rows)
		{
			if (row.parent == null && row.id != "new" && row.id != "none")
			{
				this.list.Add(row);
			}
		}
		this.list.Refresh(false);
	}

	public Card container;

	public UIList list;

	public Window.SaveData data;

	public bool needRefresh;
}
