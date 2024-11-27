using System;
using System.Collections.Generic;

public class Zone_TestRoom : Zone
{
	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		int num = EClass._map.bounds.x;
		int num2 = EClass._map.bounds.z;
		List<Card> list = new List<Card>();
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			Thing item = ThingGen.Create(row.id, -1, -1);
			list.Add(item);
		}
		foreach (Card card in list)
		{
			EClass._zone.AddCard(card, num, num2);
			card.ignoreAutoPick = true;
			num++;
			if (num >= EClass._map.bounds.maxX)
			{
				num2++;
				if (num2 >= EClass._map.bounds.maxZ)
				{
					break;
				}
				num = EClass._map.bounds.x;
			}
		}
	}
}
