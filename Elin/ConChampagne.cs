using System;
using Newtonsoft.Json;
using UnityEngine;

public class ConChampagne : BaseBuff
{
	public override void Tick()
	{
		this.count--;
		if (this.count <= 0)
		{
			this.count += 15;
			this.owner.Talk("champagne", null, null, false);
			int num = Mathf.Max(EClass.curve(this.owner.CHA * 10, 400, 100, 75), 100);
			if (EClass._zone.IsUserZone && !this.owner.IsPCFactionOrMinion && num > 500)
			{
				num = 500;
			}
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara != this.owner && chara.IsNeutralOrAbove() && this.owner.Dist(chara) <= 10)
				{
					chara.AddCondition<ConHero>(num, false);
					chara.AddCondition<ConEuphoric>(num, false);
					chara.AddCondition<ConSeeInvisible>(num, false);
				}
			}
		}
		base.Mod(-1, false);
	}

	[JsonProperty]
	public int count;
}
