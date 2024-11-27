using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ActPray : Act
{
	public override TargetType TargetType
	{
		get
		{
			return TargetType.Self;
		}
	}

	public override bool LocalAct
	{
		get
		{
			return false;
		}
	}

	public override bool Perform()
	{
		return ActPray.TryPray(Act.CC, false);
	}

	public static bool TryPray(Chara c, bool passive = false)
	{
		if (!c.HasCondition<ConWrath>())
		{
			Thing thing = c.things.Find<TraitPunishBall>();
			if (thing != null)
			{
				thing.Destroy();
				c.PlaySound("pray", 1f, true);
				c.PlayEffect("revive", true, 0f, default(Vector3));
				c.Say("piety2", c, null, null);
				return true;
			}
		}
		if (c.faith.IsEyth)
		{
			if (passive)
			{
				if (c.Evalue(1655) >= 2 && EClass.pc.party.members.Count > 1)
				{
					using (List<Chara>.Enumerator enumerator = EClass.pc.party.members.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Chara chara = enumerator.Current;
							if (chara != EClass.pc)
							{
								chara.Say("pray2", chara, chara.faith.Name, null);
								chara.ModExp(306, 200);
							}
						}
						return true;
					}
				}
				return true;
			}
			c.Say("pray", c, null, null);
			c.PlaySound("pray_ignore", 1f, true);
			return true;
		}
		foreach (Chara chara2 in EClass.pc.party.members)
		{
			if (chara2 == EClass.pc || (passive && c.Evalue(1655) >= 2))
			{
				chara2.Say("pray2", chara2, chara2.faith.Name, null);
			}
		}
		if (passive || !c.faith.TryGetGift())
		{
			if (c.IsPC && EClass.player.prayed)
			{
				if (!passive)
				{
					c.Say("pray_ignore", c, c.faith.Name, null);
					c.PlaySound("pray_ignore", 1f, true);
				}
			}
			else
			{
				ActPray.Pray(c, passive && EClass._zone.IsRegion);
			}
		}
		return true;
	}

	public static void Pray(Chara c, bool passive)
	{
		if (c.IsPC)
		{
			EClass.player.prayed = true;
		}
		if (!passive)
		{
			c.Say("pray_answer", c, Act.CC.faith.Name, null);
			c.faith.Revelation((EClass.rnd(2) == 0) ? "random" : "chat", 100);
		}
		foreach (Chara chara in EClass.pc.party.members)
		{
			if (chara == EClass.pc || (passive && c.Evalue(1655) >= 2))
			{
				chara.ModExp(306, 200);
			}
		}
		if (!passive)
		{
			c.PlaySound("pray", 1f, true);
			c.Say("pray_heal", c, null, null);
		}
		if (c.IsPC)
		{
			using (List<Chara>.Enumerator enumerator = c.party.members.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chara c2 = enumerator.Current;
					ActPray.<Pray>g__Heal|6_0(c2);
				}
				return;
			}
		}
		ActPray.<Pray>g__Heal|6_0(c);
	}

	[CompilerGenerated]
	internal static void <Pray>g__Heal|6_0(Chara _c)
	{
		_c.PlayEffect("revive", true, 0f, default(Vector3));
		_c.HealHP(999999, HealSource.None);
		_c.mana.Mod(999999);
		_c.Cure(CureType.Prayer, 999999, BlessedState.Normal);
	}
}
