public class AM_MiniGame : ActionMode
{
	public override bool ShowActionHint => false;

	public override bool ShowMouseoverTarget => false;

	public override bool AllowGeneralInput => false;

	public override bool AllowHotbar => false;

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.None;
}
