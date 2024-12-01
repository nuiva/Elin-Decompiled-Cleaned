public class InspectGroupTask : InspectGroup<TaskPoint>
{
	public override string MultiName => "Task";

	public override void OnSetActions()
	{
		TaskPoint firstTarget = base.FirstTarget;
		Add("cancel".lang() + "\n(" + firstTarget.Name + ")", "", delegate(TaskPoint t)
		{
			t.Destroy();
		}, sound: true);
	}
}
