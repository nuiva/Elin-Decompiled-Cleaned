using System;

public class InspectGroupBlock : InspectGroup<BlockInfo>
{
	public override string MultiName
	{
		get
		{
			return "Block";
		}
	}

	public override void OnSetActions()
	{
		BlockInfo firstTarget = base.FirstTarget;
		CellDetail detail = firstTarget.pos.detail;
		TaskDesignation taskDesignation = (detail != null) ? detail.designation : null;
		if (taskDesignation is TaskMine)
		{
			base.Add("cancel".lang() + "\n(" + taskDesignation.Name + ")", "", delegate(BlockInfo t)
			{
				CellDetail detail2 = t.pos.detail;
				TaskDesignation taskDesignation2 = (detail2 != null) ? detail2.designation : null;
				if (taskDesignation2 is TaskMine)
				{
					taskDesignation2.Destroy();
				}
			}, true, 0, false);
		}
		else
		{
			base.Add("Mine", "", delegate(BlockInfo t)
			{
				Point pos = t.pos.Copy();
				if (!EClass._map.tasks.designations.mine.TryAdd(new TaskMine
				{
					pos = pos
				}) && this.Solo)
				{
					SE.Beep();
				}
			}, true, 0, false);
		}
		AM_Picker.Result r = ActionMode.Picker.TestBlock(firstTarget.pos);
		if (r.IsValid)
		{
			base.Add("Copy", "", delegate()
			{
				ActionMode.Picker.Select(r);
			}, false, 0, false);
		}
	}
}
