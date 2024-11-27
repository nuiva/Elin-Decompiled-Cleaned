using System;
using System.Collections.Generic;
using System.Linq;

public class ActSummonGuard : Act
{
	public override TargetType TargetType
	{
		get
		{
			return TargetType.Chara;
		}
	}

	public override bool Perform()
	{
		List<Chara> list = (from c in EClass._map.charas
		where c.trait is TraitGuard
		select c).ToList<Chara>();
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem<Chara>();
			Act.CC.Say("calledGuard", Act.CC, null, null);
			chara.Teleport(Act.TC.pos.GetNearestPoint(false, false, true, false), false, false);
			chara.PlaySound("teleport_guard", 1f, true);
			chara.DoHostileAction(Act.TC, false);
			Effect.Get("smoke").Play(chara.pos, 0f, null, null);
			Effect.Get("smoke").Play(chara.pos, 0f, null, null);
		}
		return true;
	}
}
