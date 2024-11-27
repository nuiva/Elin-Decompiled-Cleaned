using System;

public class AM_ViewZone : AM_BaseSim
{
	public PointTarget mouseTarget
	{
		get
		{
			return EClass.scene.mouseTarget;
		}
	}

	public override AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.None;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return false;
		}
	}

	public override bool FixFocus
	{
		get
		{
			return this._FixFocus;
		}
	}

	public override void OnActivate()
	{
		ActionMode.DefaultMode = this;
		this._FixFocus = false;
		this._SyncScroll = false;
		if (WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.Hide(false);
		}
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (this.mouseTarget.drawHighlight)
		{
			base.OnRenderTile(this.mouseTarget.pos, HitResult.Default, dir);
		}
	}

	public override void OnScroll()
	{
		if (EInput.rightMouse.pressedTimer > 0.15f)
		{
			this._SyncScroll = false;
		}
	}

	public override void _OnUpdateInput()
	{
		if (EInput.rightMouse.clicked)
		{
			EClass.player.MoveZone(EClass.pc.currentZone);
		}
	}

	public bool _FixFocus;

	public bool _SyncScroll;
}
