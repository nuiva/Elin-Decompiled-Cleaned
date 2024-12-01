using UnityEngine;

public class TCUI : TC
{
	private RectTransform _rect;

	protected Vector3 lastPos;

	public override bool isUI => true;

	public override Vector3 FixPos => TC._setting.textPos;

	protected virtual void Awake()
	{
		_rect = this.Rect();
	}

	public override void OnDraw(ref Vector3 pos)
	{
		GameObject go = base.gameObject;
		Vector3 _pos = pos;
		EMono.core.actionsLateUpdate.Add(delegate
		{
			if (go != null)
			{
				lastPos = _pos;
				Vector3 position = Camera.main.WorldToScreenPoint(_pos);
				position.z = 0f;
				position += FixPos * EMono.screen.Zoom;
				_rect.position = position;
			}
		});
	}

	public void DrawImmediate(ref Vector3 pos)
	{
		Vector3 position = Camera.main.WorldToScreenPoint(pos);
		position.z = 0f;
		position += FixPos * EMono.screen.Zoom;
		_rect.position = position;
	}
}
