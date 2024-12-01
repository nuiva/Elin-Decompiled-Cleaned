using System;

public class InspectGroupArea : InspectGroup<Area>
{
	public override string MultiName => "Area";

	public override void OnSetActions()
	{
		Area first = base.FirstTarget;
		Add("expandArea", "", (Action)delegate
		{
			ActionMode.ExpandArea.Activate(first);
		}, sound: false, 0, auto: false);
		Add("shrinkArea", "", (Action)delegate
		{
			ActionMode.ExpandArea.Activate(first, _shrink: true);
		}, sound: false, 0, auto: false);
		Add("delete", "", (Action)delegate
		{
			SE.Play("trash");
			EClass._map.rooms.RemoveArea(first);
		}, sound: false, 0, auto: false);
	}
}
