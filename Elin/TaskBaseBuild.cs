using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TaskBaseBuild : TaskDesignation
{
	public bool CanPlaceCard(Point pos, Card c)
	{
		TaskBaseBuild.<>c__DisplayClass0_0 CS$<>8__locals1;
		CS$<>8__locals1.c = c;
		CS$<>8__locals1.pos = pos;
		return TaskBaseBuild.<CanPlaceCard>g__InstallCheck|0_0<TraitBed>(ref CS$<>8__locals1);
	}

	[CompilerGenerated]
	internal static bool <CanPlaceCard>g__InstallCheck|0_0<T>(ref TaskBaseBuild.<>c__DisplayClass0_0 A_0) where T : Trait
	{
		if (!(A_0.c.trait is T))
		{
			return true;
		}
		using (List<Thing>.Enumerator enumerator = A_0.pos.Things.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.trait is T)
				{
					return false;
				}
			}
		}
		return true;
	}
}
