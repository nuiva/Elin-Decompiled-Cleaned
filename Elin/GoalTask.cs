using System;
using System.Collections.Generic;

public class GoalTask : Goal
{
	public TaskManager.Designations Designations
	{
		get
		{
			return EClass._map.tasks.designations;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.manual)
		{
			yield return base.Do(this.task, null);
			yield return base.Success(null);
		}
		if (EClass.rnd(2) == 0)
		{
			yield return base.Do(new AI_Clean(), null);
		}
		if (EClass.rnd(3) == 0 && EClass._map.props.deconstructing.Count > 0)
		{
			yield return base.Do(new AI_Deconstruct(), null);
		}
		this.area = null;
		this.taskList = null;
		if (!this.TryAssignDesignations())
		{
			yield return AIAct.Status.Running;
			if (!this.TryAssignAreaTask())
			{
				yield return this.Cancel();
			}
		}
		do
		{
			yield return base.Do(this.task, null);
		}
		while (this.taskList != null && (this.TryAssignTask(1) || this.TryAssignTask(3) || this.TryAssignTask(9)));
		yield break;
	}

	public bool TryAssignAreaTask()
	{
		foreach (Area area in EClass._map.rooms.listArea)
		{
			if (this.TryAssignAreaTask(area))
			{
				this.area = area;
				this.taskList = area.taskList;
				return true;
			}
		}
		return false;
	}

	public bool TryAssignAreaTask(Area a)
	{
		this.task = a.taskList.GetTask(this.owner, -1);
		return this.task != null;
	}

	public bool TryAssignDesignations()
	{
		TaskManager.Designations designations = this.Designations;
		return (EClass.rnd(2) == 0 && this.TryAssignTask(designations.moveInstalled)) || (EClass.rnd(2) == 0 && this.TryAssignTask(designations.cut)) || (EClass.rnd(2) == 0 && this.TryAssignTask(designations.harvest)) || (EClass.rnd(2) == 0 && this.TryAssignTask(designations.mine)) || (EClass.rnd(2) == 0 && this.TryAssignTask(designations.dig)) || (EClass.rnd(2) == 0 && this.TryAssignTask(designations.build));
	}

	public bool TryAssignTask(TaskList list)
	{
		this.task = list.GetTask(this.owner, -1);
		if (this.task != null)
		{
			this.taskList = list;
			return true;
		}
		return false;
	}

	public bool TryAssignTask(int radius)
	{
		if (this.area == null)
		{
			this.task = this.taskList.GetTask(this.owner, radius);
			return this.task != null;
		}
		if (this.area.isDestroyed)
		{
			return false;
		}
		this.task = this.taskList.GetTask(this.owner, radius);
		return this.task != null;
	}

	public Task task;

	public Area area;

	public bool manual;

	public TaskList taskList;
}
