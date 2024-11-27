using System;
using TMPro;
using UnityEngine;

public class TCOrbitTask : TCOrbit
{
	public unsafe void SetOwner(TaskDesignation _task)
	{
		this.task = _task;
		base.transform.position = *this.task.pos.Position();
		this.UpdateText();
	}

	public override void Refresh()
	{
		float num = Mathf.Clamp(1f / EMono.screen.Zoom, 0f, 1.5f);
		this.text.transform.localScale = new Vector3(num, num, 1f);
		this.text.enabled = (EMono.screen.Zoom > 0.4f);
	}

	public void UpdateText()
	{
		this.text.text = this.task.GetTextOrbit();
	}

	public void UpdateText(string s)
	{
		this.text.text = s;
	}

	public TaskDesignation task;

	public TextMeshPro text;
}
