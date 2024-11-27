using System;
using System.Collections.Generic;

public class TraitTeleporter : TraitNewZone
{
	public string id
	{
		get
		{
			return this.owner.GetStr(31, null);
		}
		set
		{
			this.owner.SetStr(31, value);
		}
	}

	public override ZoneTransition.EnterState enterState
	{
		get
		{
			return ZoneTransition.EnterState.Teleport;
		}
	}

	public override string langOnUse
	{
		get
		{
			return "actUse";
		}
	}

	public override bool IsTeleport
	{
		get
		{
			return true;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return true;
		}
	}

	public override bool CanBeOnlyBuiltInHome
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.input == ActInput.AllAction && (EClass.debug.enable || !this.owner.isNPCProperty))
		{
			p.TrySetAct("actSetTeleporterID", delegate()
			{
				Dialog.InputName("dialogTeleportId", this.id.IsEmpty(""), delegate(bool cancel, string text)
				{
					if (!cancel)
					{
						this.id = text;
						EClass.game.teleports.SetID(this, EClass._zone.uid);
					}
				}, Dialog.InputType.Default);
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		if (state == PlaceState.installed)
		{
			EClass.game.teleports.SetID(this, EClass._zone.uid);
			return;
		}
		EClass.game.teleports.Remove(this.owner.uid);
	}

	public override bool TryTeleport()
	{
		if (this.id.IsEmpty() && !base.GetParam(1, null).IsEmpty())
		{
			return false;
		}
		if (this.teleportedTurn == EClass.pc.turn)
		{
			this.teleportedTurn = 0;
			return true;
		}
		List<TraitTeleporter> list = new List<TraitTeleporter>();
		foreach (Thing thing in EClass._map.things)
		{
			TraitTeleporter traitTeleporter = thing.trait as TraitTeleporter;
			if (traitTeleporter != null && traitTeleporter != this && traitTeleporter.owner.IsInstalled && traitTeleporter.owner.pos.IsInBounds && traitTeleporter.IsOn && traitTeleporter.id == this.id && traitTeleporter != this)
			{
				list.Add(traitTeleporter);
			}
		}
		if (list.Count > 0)
		{
			TraitTeleporter traitTeleporter2 = list.RandomItem<TraitTeleporter>();
			traitTeleporter2.teleportedTurn = EClass.pc.turn;
			EClass.pc.Teleport(traitTeleporter2.owner.pos, false, true);
			return true;
		}
		Zone zone = EClass.game.teleports.GetTeleportZone(this);
		if (zone is Zone_Tent || EClass._zone is Zone_Tent)
		{
			zone = null;
		}
		if (zone == null)
		{
			return false;
		}
		if ((EClass._zone.IsPCFaction && !EClass._zone.branch.HasNetwork) || (zone.IsPCFaction && !zone.branch.HasNetwork))
		{
			Msg.Say("noNetwork");
			return false;
		}
		if (EClass._zone.IsPCFaction || zone.IsPCFaction)
		{
			bool flag = true;
			using (List<Quest>.Enumerator enumerator2 = EClass.game.quests.list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.ForbidTeleport)
					{
						flag = false;
					}
				}
			}
			if (!flag)
			{
				Msg.Say("hasInvalidQuest");
				return false;
			}
		}
		EClass.pc.MoveZone(zone, new ZoneTransition
		{
			state = this.enterState,
			idTele = this.id.IsEmpty(base.GetParam(3, null))
		});
		return true;
	}

	public override void SetName(ref string s)
	{
		if (!this.id.IsEmpty())
		{
			s = "_engraved".lang(this.id, s, null, null, null);
		}
	}

	public int teleportedTurn;
}
