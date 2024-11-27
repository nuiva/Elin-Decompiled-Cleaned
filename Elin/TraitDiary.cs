using System;
using UnityEngine;

public class TraitDiary : TraitScroll
{
	public override void OnRead(Chara c)
	{
		if (!c.IsPC)
		{
			c.SayNothingHappans();
			return;
		}
		Msg.Say("diary_" + base.GetParam(1, null));
		Chara chara = CharaGen.Create(base.GetParam(1, null), -1);
		EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(false, false, true, false));
		chara.MakeAlly(false);
		chara.PlaySound("identify", 1f, true);
		chara.PlayEffect("teleport", true, 0f, default(Vector3));
		this.owner.ModNum(-1, true);
	}
}
