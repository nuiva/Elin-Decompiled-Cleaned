public class LayerDistribution : ELayer
{
	public Card container;

	public UIList list;

	public Window.SaveData data;

	public void SetContainer(Card t, Window.SaveData d)
	{
		container = t;
		data = d;
		list.callbacks = new UIList.Callback<SourceCategory.Row, ButtonCategory>
		{
			onClick = delegate(SourceCategory.Row a, ButtonCategory b)
			{
				if (a.children.Count > 0)
				{
					b.buttonFold.onClick.Invoke();
				}
			},
			onInstantiate = delegate(SourceCategory.Row a, ButtonCategory b)
			{
				b.mainText.text = a.GetName().ToTitleCase();
				b.SetFold(a.children.Count > 0, a.parent == null, delegate(UIList l)
				{
					foreach (SourceCategory.Row child in a.children)
					{
						l.Add(child);
					}
				});
			},
			onRefresh = null
		};
		foreach (SourceCategory.Row row in ELayer.sources.categories.rows)
		{
			if (row.parent == null && row.id != "new" && row.id != "none")
			{
				list.Add(row);
			}
		}
		list.Refresh();
	}
}
