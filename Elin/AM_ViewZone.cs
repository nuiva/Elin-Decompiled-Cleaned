public class AM_ViewZone : AM_BaseSim
{
	public bool _FixFocus;

	public bool _SyncScroll;

	public PointTarget mouseTarget => EClass.scene.mouseTarget;

	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.None;

	public override bool ShowMouseoverTarget => false;

	public override bool FixFocus => _FixFocus;

	public override void OnActivate()
	{
		ActionMode.DefaultMode = this;
		_FixFocus = false;
		_SyncScroll = false;
		if ((bool)WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.Hide();
		}
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (mouseTarget.drawHighlight)
		{
			base.OnRenderTile(mouseTarget.pos, HitResult.Default, dir);
		}
	}

	public override void OnScroll()
	{
		if (EInput.rightMouse.pressedTimer > 0.15f)
		{
			_SyncScroll = false;
		}
	}

	public override void _OnUpdateInput()
	{
		if (EInput.rightMouse.clicked)
		{
			EClass.player.MoveZone(EClass.pc.currentZone);
		}
	}
}
