using System;
using UnityEngine;

public class TCUI : TC
{
	public override bool isUI
	{
		get
		{
			return true;
		}
	}

	public override Vector3 FixPos
	{
		get
		{
			return TC._setting.textPos;
		}
	}

	protected virtual void Awake()
	{
		this._rect = this.Rect();
	}

	public override void OnDraw(ref Vector3 pos)
	{
		GameObject go = base.gameObject;
		Vector3 _pos = pos;
		EMono.core.actionsLateUpdate.Add(delegate
		{
			if (go != null)
			{
				this.lastPos = _pos;
				Vector3 vector = Camera.main.WorldToScreenPoint(_pos);
				vector.z = 0f;
				vector += this.FixPos * EMono.screen.Zoom;
				this._rect.position = vector;
			}
		});
	}

	public void DrawImmediate(ref Vector3 pos)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(pos);
		vector.z = 0f;
		vector += this.FixPos * EMono.screen.Zoom;
		this._rect.position = vector;
	}

	private RectTransform _rect;

	protected Vector3 lastPos;
}
