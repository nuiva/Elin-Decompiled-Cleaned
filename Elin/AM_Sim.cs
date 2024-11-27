using System;

public class AM_Sim : AM_ViewZone
{
	public override bool AllowBuildModeShortcuts
	{
		get
		{
			return true;
		}
	}

	public override bool AllowMiddleClickFunc
	{
		get
		{
			return true;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return true;
		}
	}

	public override void _OnUpdateInput()
	{
		if (EInput.leftMouse.draggedOverMargin)
		{
			if (!EClass.ui.wasActive && EClass.scene.actionMode != ActionMode.Select && Scene.ClickPoint.IsValid)
			{
				ActionMode.Select.Activate(true, false);
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
