using System;

public class TraitSpellbook : TraitBaseSpellbook
{
	public override int Difficulty
	{
		get
		{
			return 10 + this.source.LV;
		}
	}
}
