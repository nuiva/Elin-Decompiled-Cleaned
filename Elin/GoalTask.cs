using System.Collections.Generic;

public class GoalTask : Goal
{
	public Task task;

	public Area area;

	public bool manual;

	public TaskList taskList;

	public TaskManager.Designations Designations => EClass._map.tasks.designations;

	public override IEnumerable<Status> Run()
	{
		if (manual)
		{
			yield return Do(task);
			yield return Success();
		}
		if (EClass.rnd(2) == 0)
		{
			yield return Do(new AI_Clean());
		}
		if (EClass.rnd(3) == 0 && EClass._map.props.deconstructing.Count > 0)
		{
			yield return Do(new AI_Deconstruct());
		}
		area = null;
		taskList = null;
		if (!TryAssignDesignations())
		{
			yield return Status.Running;
			if (!TryAssignAreaTask())
			{
				yield return Cancel();
			}
		}
		do
		{
			yield return Do(task);
		}
		while (taskList != null && (TryAssignTask(1) || TryAssignTask(3) || TryAssignTask(9)));
	}

	public bool TryAssignAreaTask()
	{
		foreach (Area item in EClass._map.rooms.listArea)
		{
			if (TryAssignAreaTask(item))
			{
				area = item;
				taskList = item.taskList;
				return true;
			}
		}
		return false;
	}

	public bool TryAssignAreaTask(Area a)
	{
		task = a.taskList.GetTask(owner);
		return task != null;
	}

	public bool TryAssignDesignations()
	{
		TaskManager.Designations designations = Designations;
		if (EClass.rnd(2) == 0 && TryAssignTask(designations.moveInstalled))
		{
			return true;
		}
		if (EClass.rnd(2) == 0 && TryAssignTask(designations.cut))
		{
			return true;
		}
		if (EClass.rnd(2) == 0 && TryAssignTask(designations.harvest))
		{
			return true;
		}
		if (EClass.rnd(2) == 0 && TryAssignTask(designations.mine))
		{
			return true;
		}
		if (EClass.rnd(2) == 0 && TryAssignTask(designations.dig))
		{
			return true;
		}
		if (EClass.rnd(2) == 0 && TryAssignTask(designations.build))
		{
			return true;
		}
		return false;
	}

	public bool TryAssignTask(TaskList list)
	{
		task = list.GetTask(owner);
		if (task != null)
		{
			taskList = list;
			return true;
		}
		return false;
	}

	public bool TryAssignTask(int radius)
	{
		if (area != null)
		{
			if (area.isDestroyed)
			{
				return false;
			}
			task = taskList.GetTask(owner, radius);
			return task != null;
		}
		task = taskList.GetTask(owner, radius);
		return task != null;
	}
}
