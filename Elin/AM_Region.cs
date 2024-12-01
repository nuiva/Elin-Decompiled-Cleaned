using System;
using UnityEngine;

public class AM_Region : AM_Adv
{
	public Point posHighlight;

	public override float TargetZoom => 0.01f * (float)EClass.game.config.regionZoom;

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		EClass.screen.tileSelector.OnRenderTile(point, result, base.ShouldHideTile);
	}

	public override void RefreshArrow()
	{
		posOrigin = CursorSystem.posOrigin;
		posArrow = CursorSystem.position;
		float num;
		if (cursorMove || EClass.pc.pos.Distance(base.hit) > 1)
		{
			num = Util.GetAngle(posArrow.x - posOrigin.x, posArrow.y - posOrigin.y) + 90f + 22.5f;
		}
		else
		{
			num = Util.GetAngle(EClass.pc.pos.x - base.hit.x, EClass.pc.pos.z - base.hit.z) - 45f - 22.5f;
			if (num < 0f)
			{
				num = 360f + num;
			}
		}
		if ((bool)WidgetUnityChan.Instance)
		{
			WidgetUnityChan.Instance.Refresh(num);
		}
		if (clickPos != Vector3.zero)
		{
			if (Vector3.Distance(Input.mousePosition, clickPos) < EClass.core.config.game.angleMargin)
			{
				return;
			}
			clickPos = Vector3.zero;
		}
		vArrow = Vector2.zero;
		int _angle = 0;
		Action<int, int, int, int> action = delegate(int x, int y, int i, int a)
		{
			vArrow.x = x;
			vArrow.y = y;
			arrowIndex = i;
			_angle = -a;
		};
		if (num < 45f || num >= 360f)
		{
			action(-1, 0, 0, 0);
		}
		else if (num < 90f)
		{
			action(-1, 1, 1, 35);
		}
		else if (num < 135f)
		{
			action(0, 1, 2, 90);
		}
		else if (num < 180f)
		{
			action(1, 1, 3, 145);
		}
		else if (num < 225f)
		{
			action(1, 0, 4, 180);
		}
		else if (num < 270f)
		{
			action(1, -1, 5, 215);
		}
		else if (num < 315f)
		{
			action(0, -1, 6, 270);
		}
		else
		{
			action(-1, -1, 7, 325);
		}
	}
}
