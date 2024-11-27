using System;
using Newtonsoft.Json;

public class GlobalGoal : EClass
{
	public void SetOwner(Chara c)
	{
		this.owner = c;
		c.global.goal = this;
	}

	public void Start()
	{
		this.OnStart();
	}

	public virtual void OnStart()
	{
	}

	public void AdvanceHour()
	{
		this.hours++;
		this.OnAdvanceHour();
	}

	public virtual void OnAdvanceHour()
	{
	}

	public void Kill()
	{
		this.owner.global.goal = null;
	}

	public void Complete()
	{
		this.OnComplete();
		this.Kill();
	}

	public virtual void OnComplete()
	{
	}

	[JsonProperty]
	public int hours;

	public Chara owner;
}
