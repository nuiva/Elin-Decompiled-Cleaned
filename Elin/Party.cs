using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Party : EClass
{
	public List<Chara> members
	{
		get
		{
			if (this._members != null)
			{
				return this._members;
			}
			return this.SetMembers();
		}
	}

	public Chara leader
	{
		get
		{
			return this.refLeader.GetAndCache(this.uidLeader);
		}
		set
		{
			this.refLeader.Set(ref this.uidLeader, value);
		}
	}

	public List<Chara> SetMembers()
	{
		this._members = new List<Chara>();
		foreach (int uid in this.uidMembers)
		{
			this.members.Add(RefChara.Get(uid));
		}
		return this._members;
	}

	public void AddMemeber(Chara c)
	{
		if (c.party == this)
		{
			return;
		}
		if (!c.IsGlobal)
		{
			Debug.LogError("exception: " + ((c != null) ? c.ToString() : null) + " is not global chara");
		}
		this.members.Add(c);
		this.uidMembers.Add(c.uid);
		c.party = this;
		c.isSale = false;
		c.SetBool(18, false);
		if (c.homeBranch != null)
		{
			c.RefreshWorkElements(c.homeBranch.elements);
			c.homeBranch.policies.Validate();
			if (c.homeBranch.owner.map != null)
			{
				c.homeBranch.owner.map.props.sales.Remove(c);
			}
		}
		WidgetRoster.SetDirty();
	}

	public void RemoveMember(Chara c)
	{
		if (c.host != null)
		{
			ActRide.Unride(c.host, c.host.parasite == c);
		}
		this.members.Remove(c);
		this.uidMembers.Remove(c.uid);
		c.party = null;
		c.SetDirtySpeed();
		c.RefreshWorkElements(null);
		WidgetRoster.SetDirty();
	}

	public void Replace(Chara c, int index)
	{
		this.members.Remove(c);
		this.uidMembers.Remove(c.uid);
		this.members.Insert(index, c);
		this.uidMembers.Insert(index, c.uid);
	}

	public void SetLeader(Chara c)
	{
		this.leader = c;
	}

	public Element GetPartySkill(int ele)
	{
		return this.GetBestSkill(ele);
	}

	public void ModExpPartySkill(int ele, int a)
	{
	}

	public Element GetBestSkill(int ele)
	{
		Element element = Element.Create(ele, 0);
		foreach (Chara chara in this.members)
		{
			if (chara.IsAliveInCurrentZone && chara.Evalue(ele) > element.Value)
			{
				element = chara.elements.GetElement(ele);
			}
		}
		return element;
	}

	public bool IsCriticallyWounded(bool includePc = false)
	{
		foreach (Chara chara in this.members)
		{
			if ((!includePc || !chara.IsPC) && chara.IsCriticallyWounded(false))
			{
				return true;
			}
		}
		return false;
	}

	public int EValue(int ele)
	{
		int num = 0;
		foreach (Chara chara in this.members)
		{
			if (chara.Evalue(ele) > num)
			{
				num = chara.Evalue(ele);
			}
		}
		return num;
	}

	public bool HasElement(int ele)
	{
		using (List<Chara>.Enumerator enumerator = this.members.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.HasElement(ele, 1))
				{
					return true;
				}
			}
		}
		return false;
	}

	public int Count()
	{
		int num = 0;
		using (List<Chara>.Enumerator enumerator = this.members.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDead)
				{
					num++;
				}
			}
		}
		return num;
	}

	[JsonProperty]
	public int uidLeader;

	[JsonProperty]
	public List<int> uidMembers = new List<int>();

	public List<Chara> _members;

	public RefChara refLeader = new RefChara();
}
