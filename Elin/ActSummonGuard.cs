using System.Collections.Generic;
using System.Linq;

public class ActSummonGuard : Act
{
	public override TargetType TargetType => TargetType.Chara;

	public override bool Perform()
	{
		List<Chara> list = EClass._map.charas.Where((Chara c) => c.trait is TraitGuard).ToList();
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem();
			Act.CC.Say("calledGuard", Act.CC);
			chara.Teleport(Act.TC.pos.GetNearestPoint(allowBlock: false, allowChara: false));
			chara.PlaySound("teleport_guard");
			chara.DoHostileAction(Act.TC);
			Effect.Get("smoke").Play(chara.pos);
			Effect.Get("smoke").Play(chara.pos);
		}
		return true;
	}
}
