using System;

public class InspectGroupThing : InspectGroup<Thing>
{
	public override string MultiName
	{
		get
		{
			return "Thing";
		}
	}

	public override void OnSetActions()
	{
		Thing first = base.FirstTarget;
		base.Add("objInfo", "", delegate()
		{
			EClass.ui.AddLayer<LayerInfo>().Set(first, false);
		}, false, 0, false);
		if (first.trait is TraitQuestBoard)
		{
			base.Add("quest", "", delegate()
			{
				EClass.ui.AddLayer<LayerQuestBoard>();
			}, false, 20, true);
			base.Add("hire", "", delegate()
			{
				EClass.ui.AddLayer<LayerHire>();
			}, false, 20, true);
		}
		if (first.trait is TraitGacha)
		{
			base.Add("gacha", "", delegate()
			{
				EClass.ui.AddLayer<LayerGacha>();
			}, false, 10, true);
		}
		if (first.trait.IsFactory)
		{
			base.Add("craft", "icon_Inspect", delegate()
			{
				EClass.ui.AddLayer<LayerCraft>().SetFactory(first);
			}, false, 100, true);
		}
		if (first.IsInstalled)
		{
			base.Add("uninstall", "", delegate()
			{
				first.SetPlaceState(PlaceState.roaming, false);
			}, false, 0, false);
		}
		base.Add("install", "", delegate()
		{
			ActionMode.Inspect.Activate(first);
		}, false, 0, false);
		if (first.isDeconstructing)
		{
			base.Add("cancel".lang() + "\n(" + "Deconstruct".lang() + ")", "", delegate(Thing t)
			{
				t.SetDeconstruct(false);
			}, true, 0, false);
		}
		else
		{
			base.Add("Deconstruct", "", delegate(Thing t)
			{
				t.SetDeconstruct(true);
			}, true, 0, false);
		}
		AM_Picker.Result r = ActionMode.Picker.TestThing(first);
		if (r.IsValid)
		{
			base.Add("Copy", "", delegate()
			{
				ActionMode.Picker.Select(r);
			}, false, 0, false);
		}
	}
}
