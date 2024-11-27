using System;
using System.Collections.Generic;

public class AIProgress : AIAct
{
	public override bool IsAutoTurn
	{
		get
		{
			return true;
		}
	}

	public override int MaxProgress
	{
		get
		{
			return 20;
		}
	}

	public override int CurrentProgress
	{
		get
		{
			return this.progress;
		}
	}

	public virtual int Interval
	{
		get
		{
			return 2;
		}
	}

	public virtual string TextOrbit
	{
		get
		{
			return (this.progress * 100 / this.MaxProgress).ToString() + "%";
		}
	}

	public virtual string TextHint
	{
		get
		{
			return null;
		}
	}

	public override bool CancelWhenMoved
	{
		get
		{
			return true;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.owner.IsPC)
		{
			ActionMode.Adv.SetTurbo(-1);
		}
		for (;;)
		{
			this.OnBeforeProgress();
			if (!this.CanProgress())
			{
				yield return this.Cancel();
			}
			if (this.progress == 0)
			{
				this.OnProgressBegin();
			}
			if (this.status != AIAct.Status.Running)
			{
				yield return this.status;
			}
			if (this.progress % this.Interval == 0)
			{
				this.OnProgress();
			}
			this.progress++;
			if (this.status != AIAct.Status.Running)
			{
				yield return this.status;
			}
			if (this.progress >= this.MaxProgress)
			{
				this.OnProgressComplete();
				yield return base.Success(null);
			}
			yield return AIAct.Status.Running;
		}
		yield break;
	}

	public virtual void OnProgressBegin()
	{
	}

	public void CompleteProgress()
	{
		this.progress = this.MaxProgress;
	}

	public int progress;
}
