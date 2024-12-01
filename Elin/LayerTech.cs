public class LayerTech : ELayer
{
	public UIList listTech;

	public UIText textHeader;

	public bool listComplete;

	public override void OnInit()
	{
		RefreshTech();
	}

	public void RefreshTech()
	{
		WindowMenu menuRight = windows[0].menuRight;
		menuRight.Clear();
		menuRight.AddButton2Line("toggle", () => (listComplete ? "toggleComplete" : "toggleIncomplete").lang(), delegate
		{
			listComplete = !listComplete;
			RefreshTech();
		});
		textHeader.text = (listComplete ? "headerResearched" : "headerResearching").lang();
		UIList list = listTech;
		list.sortMode = ELayer.player.pref.sortResearch;
		list.callbacks = new UIList.Callback<ResearchPlan, ItemResearch>
		{
			onInstantiate = delegate(ResearchPlan a, ItemResearch b)
			{
				b.SetPlan(a, list, ELayer.Branch, this);
			},
			onList = delegate
			{
				foreach (ResearchPlan item in listComplete ? ELayer.Branch.researches.finished : ELayer.Branch.researches.plans)
				{
					list.Add(item);
				}
			},
			onSort = (ResearchPlan a, UIList.SortMode m) => a.GetSortVal(m)
		};
		list.List();
		RefreshFocus();
	}

	public void RefreshFocus()
	{
		ItemResearch[] componentsInChildren = listTech.GetComponentsInChildren<ItemResearch>();
		foreach (ItemResearch itemResearch in componentsInChildren)
		{
			itemResearch.goFocus.SetActive(itemResearch.plan == ELayer.Branch.researches.focused);
		}
	}
}
