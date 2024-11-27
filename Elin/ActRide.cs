using System;
using System.Collections.Generic;

public class ActRide : Ability
{
	public virtual bool IsParasite
	{
		get
		{
			return false;
		}
	}

	public override bool Perform()
	{
		List<Chara> list = Act.TP.ListCharas();
		list.Reverse();
		bool flag = false;
		foreach (Chara chara in list)
		{
			if (chara.host == null && !chara.IsMultisize)
			{
				if (chara == Act.CC)
				{
					if ((this.IsParasite && Act.CC.parasite != null) || (!this.IsParasite && Act.CC.ride != null))
					{
						ActRide.Unride(Act.CC, this.IsParasite);
						flag = true;
						break;
					}
				}
				else
				{
					if (!chara.trait.CanJoinPartyResident)
					{
						Msg.Say("ride_req");
						return false;
					}
					if (chara.memberType == FactionMemberType.Default && (EClass.debug.enable || (chara.IsPCFaction && chara.trait.CanJoinParty)))
					{
						ActRide.Ride(Act.CC, chara, this.IsParasite);
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			Msg.Say("noTargetFound");
		}
		return false;
	}

	public static void Ride(Chara host, Chara t, bool parasite = false)
	{
		if (parasite)
		{
			if (host.parasite != null)
			{
				ActRide.Unride(host, true);
			}
			host.parasite = t;
			host.Say("parasite", host, t, null, null);
		}
		else
		{
			if (host.ride != null)
			{
				ActRide.Unride(host, false);
			}
			host.ride = t;
			host.Say("ride", host, t, null, null);
		}
		if (!t.IsPCFaction)
		{
			t.MakeAlly(true);
		}
		EClass.pc.party.AddMemeber(t);
		if (!parasite)
		{
			if (t.race.tag.Contains("ride"))
			{
				Msg.Say("ride_good");
			}
			if (t.race.tag.Contains("noRide"))
			{
				Msg.Say("ride_bad");
			}
			if (host.HasElement(1417, 1) && t.HasCondition<ConTransmuteBroom>())
			{
				Msg.Say("ride_broom");
			}
		}
		t.host = host;
		t._CreateRenderer();
		host.PlaySound("ride", 1f, true);
		t.Talk(parasite ? "parasite" : "ride", null, null, false);
		host.SetDirtySpeed();
		t.SetDirtySpeed();
		host.SyncRide();
		t.noMove = false;
		host.Refresh(false);
	}

	public static void Unride(Chara host, bool parasite = false)
	{
		Chara chara;
		if (parasite)
		{
			chara = host.parasite;
			host.parasite = null;
			host.Say("parasite_unride", host, chara, null, null);
		}
		else
		{
			chara = host.ride;
			host.ride = null;
			host.Say("ride_unride", host, chara, null, null);
		}
		chara.host = null;
		chara._CreateRenderer();
		chara.Talk(parasite ? "parasite_unride" : "ride_unride", null, null, true);
		host.PlaySound("ride", 1f, true);
		host.SetDirtySpeed();
		chara.SetDirtySpeed();
		host.Refresh(false);
	}
}
