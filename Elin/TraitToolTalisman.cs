public class TraitToolTalisman : TraitCrafter
{
	public override string IdSource => "Talisman";

	public override string CrafterTitle => "actWrite";

	public override string idSoundProgress => "write";

	public override string idSoundComplete => "intonation";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override bool CanUseFromInventory => true;

	public override bool CloseOnComplete => true;

	public override bool IsConsumeIng => false;

	public override int numIng => 2;

	public override bool ShouldConsumeIng(SourceRecipe.Row item, int index)
	{
		return index == 1;
	}

	public override bool IsIngredient(string cat, Card c)
	{
		if (cat == "spellbook")
		{
			if (c.trait is TraitSpellbook traitSpellbook && traitSpellbook.source.abilityType.Length >= 1)
			{
				string text = traitSpellbook.source.abilityType[0];
				if (text == "attack" || text == "attackArea")
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}
}
