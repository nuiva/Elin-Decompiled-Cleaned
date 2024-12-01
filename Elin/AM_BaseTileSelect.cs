public class AM_BaseTileSelect : ActionMode
{
	public override CursorInfo DefaultCursor => CursorSystem.Select;

	public override bool enableMouseInfo => true;

	public override bool ShowMouseoverTarget => false;

	public override bool CanSelectTile => true;

	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.Build;

	public TaskManager.Designations Designations => EClass._map.tasks.designations;

	public override string idSound => "actionMode";

	public override bool AllowMiddleClickFunc => true;

	public override bool ShowMaskedThings => true;
}
