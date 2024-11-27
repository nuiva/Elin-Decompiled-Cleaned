using System;
using UnityEngine;

public class WidgetUnityChan : Widget
{
	public override void OnActivate()
	{
		WidgetUnityChan.Instance = this;
		this.goWorld = UnityEngine.Object.Instantiate<GameObject>(this.prefabWorld);
		this.animator = this.goWorld.GetComponentInChildren<Animator>();
		this.cam = this.goWorld.GetComponentInChildren<Camera>();
	}

	public void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.goWorld);
	}

	public void Refresh(float angle)
	{
		this.animator.transform.localEulerAngles = new Vector3(0f, angle + this.angleFix, 0f);
		this.cam.fieldOfView = (this.up ? 6f : 12f);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddToggle("Up", this.up, delegate(bool a)
		{
			this.up = a;
		});
		base.SetBaseContextMenu(m);
	}

	public static WidgetUnityChan Instance;

	public float angleFix;

	public bool up;

	public GameObject goWorld;

	public GameObject prefabWorld;

	public Animator animator;

	public Camera cam;

	public Vector3 scroll;
}
