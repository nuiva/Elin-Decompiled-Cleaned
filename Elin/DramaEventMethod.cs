using System;
using UnityEngine;

public class DramaEventMethod : DramaEvent
{
	public DramaEventMethod(Action _action, float _duration = 0f, bool _halt = false)
	{
		this.action = _action;
		this.time = _duration;
		this.halt = _halt;
	}

	public override bool Play()
	{
		if (this.progress == 0)
		{
			if (this.hideDialog)
			{
				this.sequence.dialog.SetActive(false);
			}
			if (this.action != null)
			{
				this.action();
			}
			if (this.time > 0f && !Input.GetKey(KeyCode.LeftControl))
			{
				TweenUtil.Tween(this.time, null, delegate()
				{
					this.progress = -1;
				});
				this.progress++;
			}
			else
			{
				this.progress = -1;
			}
		}
		if (EClass.ui.GetTopLayer() != base.layer)
		{
			if (this.sequence.dialog)
			{
				this.sequence.dialog.SetActive(false);
			}
			return false;
		}
		if (this.progress == -1 || !this.halt || (this.endFunc != null && this.endFunc()))
		{
			if (this.jumpFunc != null)
			{
				string text = this.jumpFunc();
				if (!text.IsEmpty())
				{
					this.sequence.Play(text);
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public Action action;

	public Func<bool> endFunc;

	public Func<string> jumpFunc;

	public bool halt;

	public bool hideDialog;
}
