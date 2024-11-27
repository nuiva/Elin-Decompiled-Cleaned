using System;

public class TraitUsuihon : TraitErohon
{
	public override bool UseSourceValue
	{
		get
		{
			return false;
		}
	}

	public override int Difficulty
	{
		get
		{
			return 30;
		}
	}

	public override TraitBaseSpellbook.Type BookType
	{
		get
		{
			return TraitBaseSpellbook.Type.Dojin;
		}
	}
}
