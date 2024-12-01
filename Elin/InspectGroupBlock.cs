using System;

public class InspectGroupBlock : InspectGroup<BlockInfo>
{
	public override string MultiName => "Block";

	public override void OnSetActions()
	{
		BlockInfo firstTarget = base.FirstTarget;
		TaskDesignation taskDesignation = firstTarget.pos.detail?.designation;
		if (taskDesignation is TaskMine)
		{
			Add("cancel".lang() + "\n(" + taskDesignation.Name + ")", "", delegate(BlockInfo t)
			{
				TaskDesignation taskDesignation2 = t.pos.detail?.designation;
				if (taskDesignation2 is TaskMine)
				{
					taskDesignation2.Destroy();
				}
			}, sound: true);
		}
		else
		{
			Add("Mine", "", delegate(BlockInfo t)
			{
				Point pos = t.pos.Copy();
				if (!EClass._map.tasks.designations.mine.TryAdd(new TaskMine
				{
					pos = pos
				}) && base.Solo)
				{
					SE.Beep();
				}
			}, sound: true);
		}
		AM_Picker.Result r = ActionMode.Picker.TestBlock(firstTarget.pos);
		if (r.IsValid)
		{
			Add("Copy", "", (Action)delegate
			{
				ActionMode.Picker.Select(r);
			}, sound: false, 0, auto: false);
		}
	}
}
