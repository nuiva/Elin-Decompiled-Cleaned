using System;

public class TraitErohon : TraitBaseSpellbook
{
	public override int Difficulty
	{
		get
		{
			SourceChara.Row row = EClass.sources.charas.map.TryGetValue(this.owner.c_idRefName, null);
			return ((row != null) ? row.LV : 0) / 2 + 10;
		}
	}

	public override TraitBaseSpellbook.Type BookType
	{
		get
		{
			return TraitBaseSpellbook.Type.Ero;
		}
	}

	public override int eleParent
	{
		get
		{
			return 75;
		}
	}
}
