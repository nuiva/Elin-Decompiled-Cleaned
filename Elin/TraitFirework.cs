using System;
using UnityEngine;

public class TraitFirework : TraitEffect
{
	public override string Path
	{
		get
		{
			return "Firework/" + this.GetID().IsEmpty("bees");
		}
	}

	public string GetID()
	{
		return EClass.core.refs.fireworks[base.id % EClass.core.refs.fireworks.Count].name;
	}

	public override void OnCreate(int lv)
	{
		base.id = EClass.rnd(EClass.core.refs.fireworks.Count);
	}

	public override bool OnUse(Chara c)
	{
		base.Proc(default(Vector3));
		this.owner.ModNum(-1, true);
		return base.OnUse(c);
	}

	public override void SetName(ref string s)
	{
		s = "_firework".lang(s, (base.id + 1).ToString() ?? "", null, null, null);
	}
}
