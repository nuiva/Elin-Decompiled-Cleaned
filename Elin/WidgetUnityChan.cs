using UnityEngine;

public class WidgetUnityChan : Widget
{
	public static WidgetUnityChan Instance;

	public float angleFix;

	public bool up;

	public GameObject goWorld;

	public GameObject prefabWorld;

	public Animator animator;

	public Camera cam;

	public Vector3 scroll;

	public override void OnActivate()
	{
		Instance = this;
		goWorld = Object.Instantiate(prefabWorld);
		animator = goWorld.GetComponentInChildren<Animator>();
		cam = goWorld.GetComponentInChildren<Camera>();
	}

	public void OnDestroy()
	{
		Object.DestroyImmediate(goWorld);
	}

	public void Refresh(float angle)
	{
		animator.transform.localEulerAngles = new Vector3(0f, angle + angleFix, 0f);
		cam.fieldOfView = (up ? 6f : 12f);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddToggle("Up", up, delegate(bool a)
		{
			up = a;
		});
		SetBaseContextMenu(m);
	}
}
