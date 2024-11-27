using System;
using System.Collections.Generic;

public class AI_Haul : AIAct
{
	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.dest.ExistsOnMap)
		{
			yield return base.DoGoto(this.dest.pos, 1, false, null);
		}
		if (!this.dest.ExistsOnMap)
		{
			yield return base.Success(null);
		}
		if (EClass._zone.TryAddThingInSharedContainer(this.dest, null, true, false, null, true))
		{
			this.owner.Say("haul", this.owner, this.dest, null, null);
			if (this.dest.id == "731")
			{
				this.owner.Talk("clean", null, null, false);
			}
			else
			{
				this.owner.Talk("haul", null, null, false);
			}
		}
		yield return base.Success(null);
		yield break;
	}

	public static Thing GetThingToClean(Chara c = null)
	{
		AI_Haul._list.Clear();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.placeState == PlaceState.roaming && !thing.isMasked && (thing.id == "731" || thing.id == "_egg" || thing.id == "egg_fertilized" || thing.id == "milk"))
			{
				AI_Haul._list.Add(thing);
			}
		}
		if (AI_Haul._list.Count == 0)
		{
			return null;
		}
		if (c != null)
		{
			Thing result = null;
			int num = 9999;
			foreach (Thing thing2 in AI_Haul._list)
			{
				int num2 = c.pos.Distance(thing2.pos);
				if (num2 < num && EClass._zone.TryAddThingInSharedContainer(thing2, null, false, false, null, true))
				{
					num = num2;
					result = thing2;
				}
			}
			return result;
		}
		AI_Haul._list.Shuffle<Thing>();
		foreach (Thing thing3 in AI_Haul._list)
		{
			if (EClass._zone.TryAddThingInSharedContainer(thing3, null, false, false, null, true))
			{
				return thing3;
			}
		}
		return null;
	}

	public static AI_Haul TryGetAI(Chara c)
	{
		Thing thingToClean = AI_Haul.GetThingToClean(c);
		if (thingToClean != null)
		{
			return new AI_Haul
			{
				dest = thingToClean
			};
		}
		return null;
	}

	public Thing dest;

	public static List<Thing> _list = new List<Thing>();
}
