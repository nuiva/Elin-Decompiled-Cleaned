using System;

public class TraitStairsLocked : TraitItem
{
	public override bool CanBeHeld
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool OnUse(Chara c)
	{
		if (!EClass._zone.CanUnlockExit && !EClass.debug.godMode)
		{
			Msg.Say("stairs_locked");
			this.owner.PlaySound("lock", 1f, true);
			return true;
		}
		Msg.Say("stairs_open", this.owner, null, null, null);
		this.owner.PlaySound("lock_open", 1f, true);
		this.owner.Destroy();
		Thing thing = ThingGen.Create(EClass._zone.biome.style.GetIdStairs(false), EClass._zone.biome.style.matStairs, -1);
		Zone.ignoreSpawnAnime = true;
		EClass._zone.AddCard(thing, this.owner.pos.x, this.owner.pos.z);
		thing.SetPlaceState(PlaceState.installed, false);
		return true;
	}
}
