using System;

public class TraitToolTalisman : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Talisman";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "actWrite";
		}
	}

	public override string idSoundProgress
	{
		get
		{
			return "write";
		}
	}

	public override string idSoundComplete
	{
		get
		{
			return "intonation";
		}
	}

	public override AnimeID IdAnimeProgress
	{
		get
		{
			return AnimeID.Shiver;
		}
	}

	public override bool CanUseFromInventory
	{
		get
		{
			return true;
		}
	}

	public override bool CloseOnComplete
	{
		get
		{
			return true;
		}
	}

	public override bool IsConsumeIng
	{
		get
		{
			return false;
		}
	}

	public override int numIng
	{
		get
		{
			return 2;
		}
	}

	public override bool ShouldConsumeIng(SourceRecipe.Row item, int index)
	{
		return index == 1;
	}

	public override bool IsIngredient(string cat, Card c)
	{
		if (cat == "spellbook")
		{
			TraitSpellbook traitSpellbook = c.trait as TraitSpellbook;
			if (traitSpellbook != null && traitSpellbook.source.abilityType.Length >= 1)
			{
				string a = traitSpellbook.source.abilityType[0];
				if (a == "attack" || a == "attackArea")
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}
}
