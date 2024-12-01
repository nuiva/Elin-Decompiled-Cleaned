using System;
using UnityEngine;

public class DramaEventMethod : DramaEvent
{
	public Action action;

	public Func<bool> endFunc;

	public Func<string> jumpFunc;

	public bool halt;

	public bool hideDialog;

	public DramaEventMethod(Action _action, float _duration = 0f, bool _halt = false)
	{
		action = _action;
		time = _duration;
		halt = _halt;
	}

	public override bool Play()
	{
		if (progress == 0)
		{
			if (hideDialog)
			{
				sequence.dialog.SetActive(enable: false);
			}
			if (action != null)
			{
				action();
			}
			if (time > 0f && !Input.GetKey(KeyCode.LeftControl))
			{
				TweenUtil.Tween(time, null, delegate
				{
					progress = -1;
				});
				progress++;
			}
			else
			{
				progress = -1;
			}
		}
		if (EClass.ui.GetTopLayer() != base.layer)
		{
			if ((bool)sequence.dialog)
			{
				sequence.dialog.SetActive(enable: false);
			}
			return false;
		}
		if (progress == -1 || !halt || (endFunc != null && endFunc()))
		{
			if (jumpFunc != null)
			{
				string text = jumpFunc();
				if (!text.IsEmpty())
				{
					sequence.Play(text);
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
