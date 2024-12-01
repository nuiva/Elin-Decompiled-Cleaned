using Newtonsoft.Json;
using UnityEngine;

public class HotItemChara : HotItem
{
	[JsonProperty]
	public int uid;

	public Chara chara => RefChara.Get(uid) ?? EClass.pc;

	public override Color SpriteColor => new Color(1f, 1f, 1f, 0.9f);

	public override Vector3 SpriteScale => new Vector3(0.8f, 0.8f, 1f);

	public override string Name => chara.Name;

	public override bool UseUIObjMaterial => true;

	public override Sprite GetSprite()
	{
		if (chara != null)
		{
			return chara.GetSprite();
		}
		return base.GetSprite();
	}

	public override bool TrySetAct(ActPlan p)
	{
		return base.TrySetAct(p);
	}
}
