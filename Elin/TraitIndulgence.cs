using System;
using UnityEngine;

public class TraitIndulgence : TraitScroll
{
	public override void OnRead(Chara c)
	{
		c.PlaySound("holyveil", 1f, true);
		c.PlayEffect("holyveil", true, 0f, default(Vector3));
		Msg.Say("skillbook_noSkill", c, null, null, null);
		if (c.IsPC)
		{
			EClass.player.ModKarma(20);
		}
		this.owner.ModNum(-1, true);
	}
}
