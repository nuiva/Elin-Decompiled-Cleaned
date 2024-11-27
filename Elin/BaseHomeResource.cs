using System;
using Newtonsoft.Json;
using UnityEngine;

public class BaseHomeResource : EClass
{
	public SourceHomeResource.Row source
	{
		get
		{
			return EClass.sources.homeResources.map[this.type.ToString()];
		}
	}

	public virtual int Min
	{
		get
		{
			return -99999999;
		}
	}

	public virtual int Max
	{
		get
		{
			return 99999999;
		}
	}

	public bool IsCurrency
	{
		get
		{
			return this.Group == BaseHomeResource.ResourceGroup.Currency;
		}
	}

	public bool IsRate
	{
		get
		{
			return this.Group == BaseHomeResource.ResourceGroup.Rate;
		}
	}

	public bool IsSkill
	{
		get
		{
			return this.Group == BaseHomeResource.ResourceGroup.Skill;
		}
	}

	public virtual bool IsAvailable
	{
		get
		{
			return true;
		}
	}

	public virtual float ExpRatio
	{
		get
		{
			return 0f;
		}
	}

	public virtual BaseHomeResource.ResourceGroup Group
	{
		get
		{
			return BaseHomeResource.ResourceGroup.Currency;
		}
	}

	public string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public Sprite Sprite
	{
		get
		{
			return SpriteSheet.Get("hr_" + this.type.ToString());
		}
	}

	public virtual T Create<T>(HomeResourceType _type, int _value) where T : BaseHomeResource
	{
		this.type = _type;
		this.value = _value;
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
		n.AddHeader(this.Name, null);
		n.AddTopic("TopicLeft", "vCurrent".lang(), this.value.ToString() ?? "");
		n.Build();
	}

	[JsonProperty]
	public HomeResourceType type;

	[JsonProperty]
	public int value;

	[JsonProperty]
	public int lastValue;

	public FactionBranch branch;

	public enum ResourceGroup
	{
		Currency,
		Skill,
		Rate,
		None
	}
}
