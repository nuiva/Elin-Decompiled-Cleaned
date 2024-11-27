using System;

public class ActWait : Act
{
	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Wait;
		}
	}

	public override bool Perform()
	{
		if (!Act.CC.IsPCParty)
		{
			return true;
		}
		if (Act.CC.IsPC)
		{
			ActWait.Search(Act.CC, true);
		}
		return true;
	}

	public static bool SearchMedal(Chara c, Point p)
	{
		if (c.isBlind)
		{
			return false;
		}
		if (p.detail == null || !c.CanSeeSimple(p))
		{
			return false;
		}
		foreach (Thing thing in p.detail.things)
		{
			if (thing.isHidden && thing.id == "medal" && !EClass._zone.IsUserZone)
			{
				thing.SetHidden(false);
				c.PlaySound("medal", 1f, true);
				Msg.Say("spotMedal", c, thing, null, null);
				return true;
			}
		}
		return false;
	}

	public static void Search(Chara c, bool manual = false)
	{
		if (c.isBlind)
		{
			return;
		}
		int num = 2 + c.Evalue(402);
		EClass._map.ForeachSphere(c.pos.x, c.pos.z, (float)num, delegate(Point p)
		{
			if (p.detail == null || !c.CanSeeSimple(p))
			{
				return;
			}
			foreach (Thing thing in p.detail.things)
			{
				if (thing.isHidden)
				{
					int num2 = EClass.pc.Dist(p);
					if (thing.id == "medal")
					{
						if (!manual || EClass._zone.IsUserZone)
						{
							continue;
						}
						if (num2 != 0)
						{
							Msg.Say("spotMedalNear");
							continue;
						}
					}
					else
					{
						if (EClass.rnd(c.Evalue(210) * 15 + 20 + c.PER) * (manual ? 2 : 1) <= EClass.rnd(EClass._zone.DangerLv * 8 + 60))
						{
							continue;
						}
						c.ModExp(210, EClass._zone.DangerLv * 3 / 2 + 100);
					}
					bool flag = thing.trait is TraitTrap;
					thing.SetHidden(false);
					if (thing.id == "medal")
					{
						c.PlaySound("medal", 1f, true);
						Msg.Say("spotMedal", c, thing, null, null);
					}
					else
					{
						if (flag)
						{
							c.PlaySound("spot_trap", 1f, true);
							if (EClass.core.config.game.haltOnSpotTrap)
							{
								EClass.player.haltMove = true;
							}
						}
						else
						{
							c.PlaySound("spot", 1f, true);
						}
						Msg.Say("spotHidden", c, thing, null, null);
					}
				}
			}
		});
	}
}
