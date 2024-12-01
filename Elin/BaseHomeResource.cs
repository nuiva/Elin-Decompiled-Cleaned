using Newtonsoft.Json;
using UnityEngine;

public class BaseHomeResource : EClass
{
	public enum ResourceGroup
	{
		Currency,
		Skill,
		Rate,
		None
	}

	[JsonProperty]
	public HomeResourceType type;

	[JsonProperty]
	public int value;

	[JsonProperty]
	public int lastValue;

	public FactionBranch branch;

	public SourceHomeResource.Row source => EClass.sources.homeResources.map[type.ToString()];

	public virtual int Min => -99999999;

	public virtual int Max => 99999999;

	public bool IsCurrency => Group == ResourceGroup.Currency;

	public bool IsRate => Group == ResourceGroup.Rate;

	public bool IsSkill => Group == ResourceGroup.Skill;

	public virtual bool IsAvailable => true;

	public virtual float ExpRatio => 0f;

	public virtual ResourceGroup Group => ResourceGroup.Currency;

	public string Name => source.GetName();

	public Sprite Sprite => SpriteSheet.Get("hr_" + type);

	public virtual T Create<T>(HomeResourceType _type, int _value) where T : BaseHomeResource
	{
		type = _type;
		value = _value;
		return this as T;
	}

	public virtual void Mod(int a, bool popText = true)
	{
	}

	public virtual void Refresh()
	{
	}

	public void OnRefreshEffect()
	{
	}

	public virtual void OnAdvanceHour()
	{
	}

	public virtual void OnAdvanceDay()
	{
	}

	public virtual void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(Name);
		n.AddTopic("TopicLeft", "vCurrent".lang(), value.ToString() ?? "");
		n.Build();
	}
}
