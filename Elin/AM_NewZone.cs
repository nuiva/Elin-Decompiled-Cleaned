public class AM_NewZone : ActionMode
{
	public override bool ShowActionHint => false;

	public override bool AllowHotbar => false;

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.None;

	public override void OnActivate()
	{
		ActionMode.DefaultMode = this;
		EClass.ui.AddLayer<LayerNewZone>();
	}
}
