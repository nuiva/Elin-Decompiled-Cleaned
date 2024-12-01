using System.Collections.Generic;

public class TraitSpotSpawn : Trait
{
	public List<Chara> children = new List<Chara>();

	public override int radius => 5;

	public override bool HaveUpdate => true;

	public override void Update()
	{
		for (int num = children.Count - 1; num >= 0; num--)
		{
			if (children[num].isDead)
			{
				children.RemoveAt(num);
			}
		}
		if (children.Count <= 5)
		{
			Point randomPoint = EClass._map.GetRandomPoint(owner.pos, radius);
			Chara chara = EClass._zone.SpawnMob(randomPoint);
			if (chara != null)
			{
				children.Add(chara);
			}
		}
	}
}
