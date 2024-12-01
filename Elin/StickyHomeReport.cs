public class StickyHomeReport : BaseSticky
{
	public override int idIcon => 6;

	public override bool RemoveOnClick => true;

	public override string GetText()
	{
		return "sticky_homeReport".lang() + "(" + EClass.world.date.month + "/" + EClass.world.date.day + ")";
	}

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerHomeReport>();
	}
}
