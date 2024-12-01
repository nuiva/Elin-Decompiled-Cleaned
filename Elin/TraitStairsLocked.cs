public class TraitStairsLocked : TraitItem
{
	public override bool CanBeHeld => false;

	public override bool CanBeDestroyed => false;

	public override bool OnUse(Chara c)
	{
		if (!EClass._zone.CanUnlockExit && !EClass.debug.godMode)
		{
			Msg.Say("stairs_locked");
			owner.PlaySound("lock");
			return true;
		}
		Msg.Say("stairs_open", owner);
		owner.PlaySound("lock_open");
		owner.Destroy();
		Thing thing = ThingGen.Create(EClass._zone.biome.style.GetIdStairs(upstairs: false), EClass._zone.biome.style.matStairs);
		Zone.ignoreSpawnAnime = true;
		EClass._zone.AddCard(thing, owner.pos.x, owner.pos.z);
		thing.SetPlaceState(PlaceState.installed);
		return true;
	}
}
