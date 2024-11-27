using System;

public class InspectGroupObj : InspectGroup<ObjInfo>
{
	public override string MultiName
	{
		get
		{
			return "Obj";
		}
	}

	public override void OnSetActions()
	{
		CellDetail detail = base.FirstTarget.pos.detail;
		TaskDesignation taskDesignation = (detail != null) ? detail.designation : null;
		if (taskDesignation is TaskCut)
		{
			base.Add("cancel".lang() + "\n(" + taskDesignation.Name + ")", "", delegate(ObjInfo t)
			{
				CellDetail detail2 = t.pos.detail;
				TaskDesignation taskDesignation2 = (detail2 != null) ? detail2.designation : null;
				if (taskDesignation2 is TaskCut)
				{
					taskDesignation2.Destroy();
				}
			}, true, 0, false);
			return;
		}
		base.Add("Cut", "", delegate(ObjInfo t)
		{
			Point pos = t.pos.Copy();
			if (!EClass._map.tasks.designations.cut.TryAdd(new TaskCut
			{
				pos = pos
			}) && base.Solo)
			{
				SE.Beep();
			}
		}, true, 0, false);
	}
}
