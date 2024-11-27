using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class PolicyManager : EClass
{
	public void SetOwner(FactionBranch _owner)
	{
		this.owner = _owner;
		foreach (Policy policy in this.list)
		{
			policy.SetOwner(this.owner);
		}
		this.RefreshEffects();
	}

	public void AddPolicy(string id)
	{
		this.AddPolicy(EClass.sources.elements.alias[id].id, true);
	}

	public Policy AddPolicy(int id, bool show = true)
	{
		if (this.owner.elements.GetElement(id) == null)
		{
			this.owner.elements.SetBase(id, 1, 0);
		}
		Policy policy = new Policy
		{
			id = id
		};
		policy.SetOwner(this.owner);
		this.list.Add(policy);
		if (show)
		{
			WidgetPopText.Say("rewardPolicy".lang(EClass.sources.elements.map[id].GetName(), null, null, null, null), FontColor.Default, null);
		}
		if (policy.source.tag.Contains("globalPolicy"))
		{
			EClass.pc.faction.AddGlobalPolicy(policy.id);
		}
		return policy;
	}

	public void Activate(int id)
	{
		foreach (Policy policy in this.list)
		{
			if (policy.id == id)
			{
				policy.active = true;
			}
		}
	}

	public void SetActive(int id, bool active)
	{
		foreach (Policy policy in this.list)
		{
			if (policy.id == id)
			{
				policy.active = active;
			}
		}
	}

	public bool IsActive(int id, int days = -1)
	{
		foreach (Policy policy in this.list)
		{
			if (policy.active && policy.id == id && (days == -1 || policy.days >= days))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPolicy(int id)
	{
		using (List<Policy>.Enumerator enumerator = this.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.id == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void OnSimulateHour(VirtualDate date)
	{
		foreach (Policy policy in this.list)
		{
			if (policy.active)
			{
				policy.OnAdvanceHour(date);
				if (date.hour == 0)
				{
					policy.days++;
				}
			}
			else
			{
				policy.days = 0;
			}
		}
	}

	public int GetValue(int id)
	{
		foreach (Policy policy in this.list)
		{
			if (policy.id == id && policy.active)
			{
				return policy.Ele.Value;
			}
		}
		return 0;
	}

	public int CurrentAP()
	{
		int num = 0;
		foreach (Policy policy in this.list)
		{
			if (policy.active)
			{
				num += policy.Cost;
			}
		}
		return num;
	}

	public void RefreshEffects()
	{
		foreach (Happiness happiness in this.owner.happiness.list)
		{
			happiness.OnRefreshEffect();
		}
		foreach (BaseHomeResource baseHomeResource in this.owner.resources.list)
		{
			baseHomeResource.OnRefreshEffect();
		}
		foreach (Policy policy in this.list)
		{
			if (policy.active)
			{
				policy.RefreshEffect(null);
			}
		}
		this.owner.resources.SetDirty();
	}

	public void Validate()
	{
	}

	[JsonProperty]
	public List<Policy> list = new List<Policy>();

	public FactionBranch owner;
}
