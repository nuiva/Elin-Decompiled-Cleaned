using TMPro;
using UnityEngine;

public class TCOrbitTask : TCOrbit
{
	public TaskDesignation task;

	public TextMeshPro text;

	public void SetOwner(TaskDesignation _task)
	{
		task = _task;
		base.transform.position = task.pos.Position();
		UpdateText();
	}

	public override void Refresh()
	{
		float num = Mathf.Clamp(1f / EMono.screen.Zoom, 0f, 1.5f);
		text.transform.localScale = new Vector3(num, num, 1f);
		text.enabled = EMono.screen.Zoom > 0.4f;
	}

	public void UpdateText()
	{
		text.text = task.GetTextOrbit();
	}

	public void UpdateText(string s)
	{
		text.text = s;
	}
}
