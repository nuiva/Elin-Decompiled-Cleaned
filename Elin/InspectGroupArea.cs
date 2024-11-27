using System;

public class InspectGroupArea : InspectGroup<Area>
{
	public override string MultiName
	{
		get
		{
			return "Area";
		}
	}

	public override void OnSetActions()
	{
		Area first = base.FirstTarget;
		base.Add("expandArea", "", delegate()
		{
			ActionMode.ExpandArea.Activate(first, false);
		}, false, 0, false);
		base.Add("shrinkArea", "", delegate()
		{
			ActionMode.ExpandArea.Activate(first, true);
		}, false, 0, false);
		base.Add("delete", "", delegate()
		{
			SE.Play("trash");
			EClass._map.rooms.RemoveArea(first);
		}, false, 0, false);
	}
}
