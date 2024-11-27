using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TaskManager : EClass
{
	public void OnLoad()
	{
		this.designations.OnLoad();
	}

	[JsonProperty]
	public TaskManager.Designations designations = new TaskManager.Designations();

	public UndoManager undo = new UndoManager();

	public class Designations : EClass
	{
		public void OnLoad()
		{
			this.mine.OnLoad();
			this.dig.OnLoad();
			this.cut.OnLoad();
			this.harvest.OnLoad();
			this.build.OnLoad();
			this.moveInstalled.OnLoad();
		}

		public bool CanRemoveDesignation(Point point)
		{
			return this.mapAll.TryGetValue(point.index, null) != null;
		}

		public void TryRemoveDesignation(Point point)
		{
			if (!this.CanRemoveDesignation(point))
			{
				return;
			}
			TaskDesignation taskDesignation = this.mapAll.TryGetValue(point.index, null);
			if (taskDesignation.owner != null)
			{
				taskDesignation.owner.SetAI(new NoGoal());
			}
			taskDesignation.Destroy();
		}

		[JsonProperty]
		public DesignationList<TaskMine> mine = new DesignationList<TaskMine>();

		[JsonProperty]
		public DesignationList<TaskDig> dig = new DesignationList<TaskDig>();

		[JsonProperty]
		public DesignationList<TaskCut> cut = new DesignationList<TaskCut>();

		[JsonProperty]
		public DesignationList<TaskHarvest> harvest = new DesignationList<TaskHarvest>();

		[JsonProperty]
		public DesignationList<TaskBuild> build = new DesignationList<TaskBuild>();

		[JsonProperty]
		public DesignationList<TaskMoveInstalled> moveInstalled = new DesignationList<TaskMoveInstalled>();

		public Dictionary<int, TaskDesignation> mapAll = new Dictionary<int, TaskDesignation>();
	}
}
