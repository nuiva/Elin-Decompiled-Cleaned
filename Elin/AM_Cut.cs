public class AM_Cut : AM_Designation<TaskCut>
{
	public override int CostMoney => 10;

	public override bool AllowAutoClick => true;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Cut);
	}

	public override void OnActivate()
	{
		list = base.Designations.cut;
		base.OnActivate();
	}
}
