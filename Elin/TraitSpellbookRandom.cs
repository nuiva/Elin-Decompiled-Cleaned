using System;

public class TraitSpellbookRandom : TraitSpellbook
{
	public override TraitBaseSpellbook.Type BookType
	{
		get
		{
			return TraitBaseSpellbook.Type.RandomSpell;
		}
	}

	public override SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.owner.refVal];
		}
	}

	public override string GetName()
	{
		return "spellbook_".lang(this.source.GetName().ToLower(), null, null, null, null);
	}
}
