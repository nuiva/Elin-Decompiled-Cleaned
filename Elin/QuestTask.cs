using Newtonsoft.Json;

public class QuestTask : EClass
{
	[JsonProperty]
	public Quest owner;

	public virtual string RefDrama1 => "";

	public virtual string RefDrama2 => "";

	public virtual string RefDrama3 => "";

	public void SetOwner(Quest quest)
	{
		owner = quest;
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
}
