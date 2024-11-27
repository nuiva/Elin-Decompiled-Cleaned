using System;

public class InspectGroupTask : InspectGroup<TaskPoint>
{
	public override string MultiName
	{
		get
		{
			return "Task";
		}
	}

	public override void OnSetActions()
	{
		TaskPoint firstTarget = base.FirstTarget;
		base.Add("cancel".lang() + "\n(" + firstTarget.Name + ")", "", delegate(TaskPoint t)
		{
			t.Destroy();
		}, true, 0, false);
	}
}
