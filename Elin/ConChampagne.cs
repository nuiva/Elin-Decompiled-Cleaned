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
			int p = Mathf.Max(EClass.curve(this.owner.CHA * 10, 400, 100, 75), 100);
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara != this.owner && chara.IsNeutralOrAbove() && this.owner.Dist(chara) <= 10)
				{
					chara.AddCondition<ConHero>(p, false);
					chara.AddCondition<ConEuphoric>(p, false);
					chara.AddCondition<ConSeeInvisible>(p, false);
				}
			}
		}
		base.Mod(-1, false);
	}

	[JsonProperty]
	public int count;
}
