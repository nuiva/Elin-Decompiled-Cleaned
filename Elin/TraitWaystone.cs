using System;

public class TraitWaystone : Trait
{
	public bool IsTemp
	{
		get
		{
			return this.owner.id == "waystone_temp";
		}
	}

	public override bool CanUse(Chara c)
	{
		return this.IsTemp;
	}

	public override bool OnUse(Chara c)
	{
		this.owner.ModNum(-1, true);
		EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
		return false;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.parent.IsRegion)
		{
			return;
		}
		if (EClass._zone.IsInstance && !EClass._zone.IsUserZone)
		{
			return;
		}
		if (EClass._zone is Zone_Dungeon || EClass._zone is Zone_Tent)
		{
			return;
		}
		p.TrySetAct("actNewZone", delegate()
		{
			if (this.IsTemp)
			{
				this.owner.ModNum(-1, true);
			}
			EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
			return false;
		}, this.owner, CursorSystem.MoveZone, 1, false, true, false);
		if (EClass._zone.IsPCFaction || EClass._zone.IsTown)
		{
			if (EClass.player.spawnZone != EClass._zone)
			{
				p.TrySetAct("actSetSpawn", delegate()
				{
					Effect.Get("aura_heaven").Play(EClass.pc.pos, 0f, null, null);
					EClass.Sound.Play("worship");
					EClass.player.spawnZone = EClass._zone;
					Msg.Say("setSpawn", this.owner, null, null, null);
					return true;
				}, this.owner, null, 1, false, true, false);
				return;
			}
			if (EClass.player.spawnZone != EClass.pc.homeZone || EClass._zone != EClass.pc.homeZone)
			{
				p.TrySetAct("actUnsetSpawn", delegate()
				{
					EClass.Sound.Play("trash");
					EClass.player.spawnZone = EClass.pc.homeZone;
					Msg.Say("unsetSpawn", this.owner, null, null, null);
					return true;
				}, this.owner, null, 1, false, true, false);
			}
		}
	}
}
