using System;
using Newtonsoft.Json;
using UnityEngine;

public class HotItemChara : HotItem
{
	public Chara chara
	{
		get
		{
			return RefChara.Get(this.uid) ?? EClass.pc;
		}
	}

	public override Color SpriteColor
	{
		get
		{
			return new Color(1f, 1f, 1f, 0.9f);
		}
	}

	public override Vector3 SpriteScale
	{
		get
		{
			return new Vector3(0.8f, 0.8f, 1f);
		}
	}

	public override string Name
	{
		get
		{
			return this.chara.Name;
		}
	}

	public override bool UseUIObjMaterial
	{
		get
		{
			return true;
		}
	}

	public override Sprite GetSprite()
	{
		if (this.chara != null)
		{
			return this.chara.GetSprite(0);
		}
		return base.GetSprite();
	}

	public override bool TrySetAct(ActPlan p)
	{
		return base.TrySetAct(p);
	}

	[JsonProperty]
	public int uid;
}
