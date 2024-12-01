using Newtonsoft.Json;

public class GlobalGoal : EClass
{
	[JsonProperty]
	public int hours;

	public Chara owner;

	public void SetOwner(Chara c)
	{
		owner = c;
		c.global.goal = this;
	}

	public void Start()
	{
		OnStart();
	}

	public virtual void OnStart()
	{
	}

	public void AdvanceHour()
	{
		hours++;
		OnAdvanceHour();
	}

	public virtual void OnAdvanceHour()
	{
	}

	public void Kill()
	{
		owner.global.goal = null;
	}

	public void Complete()
	{
		OnComplete();
		Kill();
	}

	public virtual void OnComplete()
	{
	}
}
