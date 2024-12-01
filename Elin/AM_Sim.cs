public class AM_Sim : AM_ViewZone
{
	public override bool AllowBuildModeShortcuts => true;

	public override bool AllowMiddleClickFunc => true;

	public override bool ShowMouseoverTarget => true;

	public override void _OnUpdateInput()
	{
		if (EInput.leftMouse.draggedOverMargin)
		{
			if (!EClass.ui.wasActive && EClass.scene.actionMode != ActionMode.Select && Scene.ClickPoint.IsValid)
			{
				ActionMode.Select.Activate();
				EClass.screen.tileSelector.lastPoint.Set(Point.Invalid);
				EClass.screen.tileSelector.start = Scene.ClickPoint.Copy();
			}
			return;
		}
		if (EInput.leftMouse.down)
		{
			Scene.ClickPoint.Set(Scene.HitPoint);
		}
		if (EInput.rightMouse.clicked)
		{
			WidgetInspector.Hide();
		}
	}
}
