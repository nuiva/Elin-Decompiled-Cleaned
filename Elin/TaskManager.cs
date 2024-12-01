using System.Collections.Generic;
using Newtonsoft.Json;

public class TaskManager : EClass
{
	public class Designations : EClass
	{
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

		public void OnLoad()
		{
			mine.OnLoad();
			dig.OnLoad();
			cut.OnLoad();
			harvest.OnLoad();
			build.OnLoad();
			moveInstalled.OnLoad();
		}

		public bool CanRemoveDesignation(Point point)
		{
			if (mapAll.TryGetValue(point.index) == null)
			{
				return false;
			}
			return true;
		}

		public void TryRemoveDesignation(Point point)
		{
			if (CanRemoveDesignation(point))
			{
				TaskDesignation taskDesignation = mapAll.TryGetValue(point.index);
				if (taskDesignation.owner != null)
				{
					taskDesignation.owner.SetAI(new NoGoal());
				}
				taskDesignation.Destroy();
			}
		}
	}

	[JsonProperty]
	public Designations designations = new Designations();

	public UndoManager undo = new UndoManager();

	public void OnLoad()
	{
		designations.OnLoad();
	}
}
