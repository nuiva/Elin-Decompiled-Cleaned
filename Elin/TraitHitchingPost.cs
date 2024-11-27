using System;

public class TraitHitchingPost : TraitFloorSwitch
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override void OnActivateTrap(Chara c)
	{
		if (!c.IsPC || !EClass._zone.IsPCFaction)
		{
			return;
		}
		if (EClass.pc.ride == null)
		{
			foreach (Chara chara in this.owner.pos.Charas)
			{
				if (chara.IsPCFaction && !chara.IsPC && !chara.IsPCParty)
				{
					ActRide.Ride(EClass.pc, chara, false);
					return;
				}
			}
			Msg.Say("noRide");
			TraitSwitch.haltMove = false;
			return;
		}
		Chara ride = EClass.pc.ride;
		ActRide.Unride(EClass.pc, false);
		EClass.pc.party.RemoveMember(ride);
		ride.noMove = true;
		ride.orgPos = new Point(this.owner.pos);
	}
}
