public class InspectGroupObj : InspectGroup<ObjInfo>
{
	public override string MultiName => "Obj";

	public override void OnSetActions()
	{
		TaskDesignation taskDesignation = base.FirstTarget.pos.detail?.designation;
		if (taskDesignation is TaskCut)
		{
			Add("cancel".lang() + "\n(" + taskDesignation.Name + ")", "", delegate(ObjInfo t)
			{
				TaskDesignation taskDesignation2 = t.pos.detail?.designation;
				if (taskDesignation2 is TaskCut)
				{
					taskDesignation2.Destroy();
				}
			}, sound: true);
			return;
		}
		Add("Cut", "", delegate(ObjInfo t)
		{
			Point pos = t.pos.Copy();
			if (!EClass._map.tasks.designations.cut.TryAdd(new TaskCut
			{
				pos = pos
			}) && base.Solo)
			{
				SE.Beep();
			}
		}, sound: true);
	}
}
