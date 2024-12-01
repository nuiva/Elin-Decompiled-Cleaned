public class TraitSpellbook : TraitBaseSpellbook
{
	public override int Difficulty => 10 + source.LV;
}
