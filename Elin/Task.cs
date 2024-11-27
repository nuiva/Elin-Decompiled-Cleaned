using System;
using Newtonsoft.Json;

public class Task : AIAct
{
	public override TargetType TargetType
	{
		get
		{
			return TargetType.Ground;
		}
	}

	public virtual HitResult GetHitResult()
	{
		return HitResult.Valid;
	}

	public override void OnSuccess()
	{
		this.Destroy();
	}

	public void TryLayer(int min = 30)
	{
		this.nextTry = EClass.game.sessionMin + min;
	}

	public AIAct.Status Destroy()
	{
		if (this.status == AIAct.Status.Running)
		{
			this.status = AIAct.Status.Fail;
		}
		if (this.isDestroyed)
		{
			return this.status;
		}
		this.isDestroyed = true;
		if (this.taskList != null)
		{
			this.taskList.Remove(this);
		}
		this.OnDestroy();
		return this.status;
	}

	public virtual void OnDestroy()
	{
	}

	public override AIAct.Status Cancel()
	{
		this.TryLayer(30);
		return base.Cancel();
	}

	public override void OnReset()
	{
		if (this.isDestroyed)
		{
			return;
		}
		if (this.taskList != null)
		{
			this.taskList.SetAstLastItem(this);
		}
	}

	public override bool CanProgress()
	{
		if (this.isDestroyed)
		{
			return false;
		}
		HitResult hitResult = this.GetHitResult();
		return hitResult == HitResult.Valid || hitResult == HitResult.Warning;
	}

	public bool CanPerformTask(Chara chara, int radius)
	{
		return !this.suspended && this._CanPerformTask(chara, radius);
	}

	public virtual bool _CanPerformTask(Chara chara, int radius)
	{
		return true;
	}

	public virtual void OnAdd()
	{
	}

	public void ToggleSuspend()
	{
		this.suspended = !this.suspended;
		if (this.suspended && this.IsRunning)
		{
			this.Cancel();
		}
	}

	public TaskList taskList;

	public bool isDestroyed;

	public int nextTry;

	[JsonProperty]
	public bool suspended;
}
