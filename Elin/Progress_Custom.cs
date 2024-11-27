using System;

public class Progress_Custom : AIProgress
{
	public override bool CancelWhenMoved
	{
		get
		{
			return this.cancelWhenMoved;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return this.cancelWhenDamaged;
		}
	}

	public override int MaxProgress
	{
		get
		{
			return this.maxProgress;
		}
	}

	public override bool ShowProgress
	{
		get
		{
			return this.showProgress;
		}
	}

	public override int Interval
	{
		get
		{
			return this.interval;
		}
	}

	public override string TextHint
	{
		get
		{
			return this.textHint;
		}
	}

	public override bool CanProgress()
	{
		Func<bool> func = this.canProgress;
		return func == null || func();
	}

	public override void OnProgress()
	{
		Action<Progress_Custom> action = this.onProgress;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	public override void OnProgressComplete()
	{
		Action action = this.onProgressComplete;
		if (action == null)
		{
			return;
		}
		action();
	}

	public override void OnBeforeProgress()
	{
		Action action = this.onBeforeProgress;
		if (action == null)
		{
			return;
		}
		action();
	}

	public override void OnProgressBegin()
	{
		Action action = this.onProgressBegin;
		if (action == null)
		{
			return;
		}
		action();
	}

	public Progress_Custom SetDuration(int max, int _interval = 2)
	{
		this.maxProgress = max;
		this.interval = _interval;
		return this;
	}

	public string textHint;

	public int maxProgress = 20;

	public int interval = 2;

	public bool cancelWhenMoved = true;

	public bool cancelWhenDamaged = true;

	public bool showProgress = true;

	public Func<bool> canProgress;

	public Action onBeforeProgress;

	public Action onProgressComplete;

	public Action onProgressBegin;

	public Action<Progress_Custom> onProgress;
}
