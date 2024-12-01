public class AM_NoMap : ActionMode
{
	public override bool ShowActionHint => false;

	public override bool AllowHotbar => false;

	public override bool AllowGeneralInput => false;

	public override bool IsNoMap => true;

	public override BaseGameScreen TargetGameScreen => EClass.scene.screenNoMap;

	public override void OnActivate()
	{
		EClass.ui.layerFloat.SetActive(enable: false);
	}

	public override void OnDeactivate()
	{
		EClass.ui.layerFloat.SetActive(enable: true);
	}
}
