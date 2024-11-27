using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class CardManager : EClass
{
	[OnDeserializing]
	private void OnDeserializing(StreamingContext context)
	{
		Debug.Log("#io CardManager OnDeserializing:");
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		Debug.Log("#io CardManager OnDeserialized:" + this.globalCharas.Count.ToString());
	}

	public void AssignUID(Card c)
	{
		c.uid = this.uidNext;
		this.uidNext++;
	}

	public void AssignUIDRecursive(Card c)
	{
		if (c.things.Count > 0)
		{
			foreach (Thing c2 in c.things)
			{
				this.AssignUIDRecursive(c2);
			}
		}
		this.AssignUID(c);
	}

	public List<Chara> ListGlobalChara(Zone z)
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in this.globalCharas.Values)
		{
			if (chara.currentZone == z)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	public Chara Find(int uid)
	{
		foreach (Chara chara in this.globalCharas.Values)
		{
			if (chara.uid == uid)
			{
				return chara;
			}
		}
		return null;
	}

	[JsonProperty]
	public CardManager.GlobalCharaList globalCharas = new CardManager.GlobalCharaList();

	[JsonProperty]
	public int uidNext = 1;

	[JsonProperty]
	public Thing container_shipping;

	[JsonProperty]
	public Thing container_deliver;

	[JsonProperty]
	public Thing container_deposit;

	[JsonProperty]
	public List<Thing> listPackage = new List<Thing>();

	[JsonProperty]
	public List<Chara> listAdv = new List<Chara>();

	public class GlobalCharaList : Dictionary<int, Chara>
	{
		public void Add(Chara c)
		{
			base[c.uid] = c;
		}

		public void Remove(Chara c)
		{
			base.Remove(c.uid);
		}

		public Chara Find(string id)
		{
			foreach (Chara chara in base.Values)
			{
				if (chara.id == id)
				{
					return chara;
				}
			}
			return null;
		}

		public Chara Find(int uid)
		{
			foreach (Chara chara in base.Values)
			{
				if (chara.uid == uid)
				{
					return chara;
				}
			}
			return null;
		}
	}
}
