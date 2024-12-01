public class StickyMenu : BaseSticky
{
	public override int idIcon => 5;

	public override string idLang => "";

	public override bool ShouldShow => base.widget.extra.showNerun;

	public override bool animate => false;

	public override void OnClick()
	{
		LayerDrama.ActivateMain("nerun", "9-1");
	}
}
