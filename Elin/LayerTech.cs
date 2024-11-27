using System;

public class LayerTech : ELayer
{
	public override void OnInit()
	{
		this.RefreshTech();
	}

	public void RefreshTech()
	{
		WindowMenu menuRight = this.windows[0].menuRight;
		menuRight.Clear();
		menuRight.AddButton2Line("toggle", () => (this.listComplete ? "toggleComplete" : "toggleIncomplete").lang(), delegate(UIButton b)
		{
			this.listComplete = !this.listComplete;
			this.RefreshTech();
		}, null, "2line");
		this.textHeader.text = (this.listComplete ? "headerResearched" : "headerResearching").lang();
		UIList list = this.listTech;
		list.sortMode = ELayer.player.pref.sortResearch;
		BaseList list2 = list;
		UIList.Callback<ResearchPlan, ItemResearch> callback = new UIList.Callback<ResearchPlan, ItemResearch>();
		callback.onInstantiate = delegate(ResearchPlan a, ItemResearch b)
		{
			b.SetPlan(a, list, ELayer.Branch, this);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (ResearchPlan o in (this.listComplete ? ELayer.Branch.researches.finished : ELayer.Branch.researches.plans))
			{
				list.Add(o);
			}
		};
		callback.onSort = ((ResearchPlan a, UIList.SortMode m) => a.GetSortVal(m));
		list2.callbacks = callback;
		list.List(false);
		this.RefreshFocus();
	}

	public void RefreshFocus()
	{
		foreach (ItemResearch itemResearch in this.listTech.GetComponentsInChildren<ItemResearch>())
		{
			itemResearch.goFocus.SetActive(itemResearch.plan == ELayer.Branch.researches.focused);
		}
	}

	public UIList listTech;

	public UIText textHeader;

	public bool listComplete;
}
