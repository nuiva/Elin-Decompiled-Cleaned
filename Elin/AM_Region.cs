using System;
using UnityEngine;

public class AM_Region : AM_Adv
{
	public override float TargetZoom
	{
		get
		{
			return 0.01f * (float)EClass.game.config.regionZoom;
		}
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		EClass.screen.tileSelector.OnRenderTile(point, result, base.ShouldHideTile);
	}

	public override void RefreshArrow()
	{
		this.posOrigin = CursorSystem.posOrigin;
		this.posArrow = CursorSystem.position;
		float num;
		if (this.cursorMove || EClass.pc.pos.Distance(base.hit) > 1)
		{
			num = Util.GetAngle(this.posArrow.x - this.posOrigin.x, this.posArrow.y - this.posOrigin.y) + 90f + 22.5f;
		}
		else
		{
			num = Util.GetAngle((float)(EClass.pc.pos.x - base.hit.x), (float)(EClass.pc.pos.z - base.hit.z)) - 45f - 22.5f;
			if (num < 0f)
			{
				num = 360f + num;
			}
		}
		if (WidgetUnityChan.Instance)
		{
			WidgetUnityChan.Instance.Refresh(num);
		}
		if (this.clickPos != Vector3.zero)
		{
			if (Vector3.Distance(Input.mousePosition, this.clickPos) < EClass.core.config.game.angleMargin)
			{
				return;
			}
			this.clickPos = Vector3.zero;
		}
		this.vArrow = Vector2.zero;
		int _angle = 0;
		Action<int, int, int, int> action = delegate(int x, int y, int i, int a)
		{
			this.vArrow.x = (float)x;
			this.vArrow.y = (float)y;
			this.arrowIndex = i;
			_angle = -a;
		};
		if (num < 45f || num >= 360f)
		{
			action(-1, 0, 0, 0);
			return;
		}
		if (num < 90f)
		{
			action(-1, 1, 1, 35);
			return;
		}
		if (num < 135f)
		{
			action(0, 1, 2, 90);
			return;
		}
		if (num < 180f)
		{
			action(1, 1, 3, 145);
			return;
		}
		if (num < 225f)
		{
			action(1, 0, 4, 180);
			return;
		}
		if (num < 270f)
		{
			action(1, -1, 5, 215);
			return;
		}
		if (num < 315f)
		{
			action(0, -1, 6, 270);
			return;
		}
		action(-1, -1, 7, 325);
	}

	public Point posHighlight;
}
