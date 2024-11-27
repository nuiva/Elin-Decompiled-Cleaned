using System;

public class LayerDistribution : ELayer
{
	public void SetContainer(Card t, Window.SaveData d)
	{
		this.container = t;
		this.data = d;
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
			b.mainText.text = a.GetName().ToTitleCase(false);
			b.SetFold(a.children.Count > 0, a.parent == null, delegate(UIList l)
			{
				foreach (SourceCategory.Row o in a.children)
				{
					l.Add(o);
				}
			});
		};
		callback.onRefresh = null;
		baseList.callbacks = callback;
		foreach (SourceCategory.Row row in ELayer.sources.categories.rows)
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
}
