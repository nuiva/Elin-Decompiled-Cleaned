using System;

public class AM_Harvest : AM_Designation<TaskHarvest>
{
	public override void OnActivate()
	{
		this.list = base.Designations.harvest;
		base.OnActivate();
	}
}
