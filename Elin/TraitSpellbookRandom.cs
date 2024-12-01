public class TraitSpellbookRandom : TraitSpellbook
{
	public override Type BookType => Type.RandomSpell;

	public override SourceElement.Row source => EClass.sources.elements.map[owner.refVal];

	public override string GetName()
	{
		return "spellbook_".lang(source.GetName().ToLower());
	}
}
