using System;
using Newtonsoft.Json;

public class QuestDestZone : QuestRandom
{
	public virtual bool IsDeliver
	{
		get
		{
			return true;
		}
	}

	public Zone DestZone
	{
		get
		{
			return RefZone.Get(this.uidDest);
		}
	}

	public override Chara DestChara
	{
		get
		{
			if (this.IsDeliver && this.DestZone == EClass._zone)
			{
				Chara chara = EClass._zone.FindChara(this.uidTarget);
				if (chara != null && chara.IsAliveInCurrentZone)
				{
					return EClass._zone.FindChara(this.uidTarget);
				}
			}
			return base.chara;
		}
	}

	public override string RefDrama3
	{
		get
		{
			if (this.DestZone != null)
			{
				return this.DestZone.Name;
			}
			Zone clientZone = base.ClientZone;
			return ((clientZone != null) ? clientZone.Name : null) ?? "???";
		}
	}

	public override string RefDrama4
	{
		get
		{
			string result;
			if (this.uidTarget != 0)
			{
				if ((result = this.DestZone.dictCitizen.TryGetValue(this.uidTarget, null)) == null)
				{
					return "???";
				}
			}
			else
			{
				result = "";
			}
			return result;
		}
	}

	public void SetDest(Zone z, int target)
	{
		this.uidDest = z.uid;
		this.uidTarget = target;
	}

	[JsonProperty]
	public int uidDest;

	[JsonProperty]
	public int uidTarget;
}
