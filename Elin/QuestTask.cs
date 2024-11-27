using System;
using Newtonsoft.Json;

public class QuestTask : EClass
{
	public void SetOwner(Quest quest)
	{
		this.owner = quest;
	}

	public virtual bool IsComplete()
	{
		return false;
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnStart()
	{
	}

	public virtual void OnKillChara(Chara c)
	{
	}

	public virtual void OnGiveItem(Chara c, Thing t)
	{
	}

	public virtual void OnModKarma(int a)
	{
	}

	public virtual string GetTextProgress()
	{
		return "";
	}

	public virtual void OnGetDetail(ref string detail, bool onJournal)
	{
	}

	public virtual string RefDrama1
	{
		get
		{
			return "";
		}
	}

	public virtual string RefDrama2
	{
		get
		{
			return "";
		}
	}

	public virtual string RefDrama3
	{
		get
		{
			return "";
		}
	}

	[JsonProperty]
	public Quest owner;
}
