public class AM_Harvest : AM_Designation<TaskHarvest>
{
	public override void OnActivate()
	{
		list = base.Designations.harvest;
		base.OnActivate();
	}
}
