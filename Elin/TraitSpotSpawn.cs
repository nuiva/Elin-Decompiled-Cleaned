using System;
using System.Collections.Generic;

public class TraitSpotSpawn : Trait
{
	public override int radius
	{
		get
		{
			return 5;
		}
	}

	public override bool HaveUpdate
	{
		get
		{
			return true;
		}
	}

	public override void Update()
	{
		for (int i = this.children.Count - 1; i >= 0; i--)
		{
			if (this.children[i].isDead)
			{
				this.children.RemoveAt(i);
			}
		}
		if (this.children.Count > 5)
		{
			return;
		}
		Point randomPoint = EClass._map.GetRandomPoint(this.owner.pos, this.radius, 100, true, true);
		Chara chara = EClass._zone.SpawnMob(randomPoint, null);
		if (chara != null)
		{
			this.children.Add(chara);
		}
	}

	public List<Chara> children = new List<Chara>();
}
