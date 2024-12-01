using System;

public class InspectGroupThing : InspectGroup<Thing>
{
	public override string MultiName => "Thing";

	public override void OnSetActions()
	{
		Thing first = base.FirstTarget;
		Add("objInfo", "", (Action)delegate
		{
			EClass.ui.AddLayer<LayerInfo>().Set(first);
		}, sound: false, 0, auto: false);
		if (first.trait is TraitQuestBoard)
		{
			Add("quest", "", (Action)delegate
			{
				EClass.ui.AddLayer<LayerQuestBoard>();
			}, sound: false, 20, auto: true);
			Add("hire", "", (Action)delegate
			{
				EClass.ui.AddLayer<LayerHire>();
			}, sound: false, 20, auto: true);
		}
		if (first.trait is TraitGacha)
		{
			Add("gacha", "", (Action)delegate
			{
				EClass.ui.AddLayer<LayerGacha>();
			}, sound: false, 10, auto: true);
		}
		if (first.trait.IsFactory)
		{
			Add("craft", "icon_Inspect", (Action)delegate
			{
				EClass.ui.AddLayer<LayerCraft>().SetFactory(first);
			}, sound: false, 100, auto: true);
		}
		if (first.IsInstalled)
		{
			Add("uninstall", "", (Action)delegate
			{
				first.SetPlaceState(PlaceState.roaming);
			}, sound: false, 0, auto: false);
		}
		Add("install", "", (Action)delegate
		{
			ActionMode.Inspect.Activate(first);
		}, sound: false, 0, auto: false);
		if (first.isDeconstructing)
		{
			Add("cancel".lang() + "\n(" + "Deconstruct".lang() + ")", "", delegate(Thing t)
			{
				t.SetDeconstruct(deconstruct: false);
			}, sound: true);
		}
		else
		{
			Add("Deconstruct", "", delegate(Thing t)
			{
				t.SetDeconstruct(deconstruct: true);
			}, sound: true);
		}
		AM_Picker.Result r = ActionMode.Picker.TestThing(first);
		if (r.IsValid)
		{
			Add("Copy", "", (Action)delegate
			{
				ActionMode.Picker.Select(r);
			}, sound: false, 0, auto: false);
		}
	}
}
