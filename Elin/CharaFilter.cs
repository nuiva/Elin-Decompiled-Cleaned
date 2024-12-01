using System;

public class CharaFilter : CardFilter
{
	public Func<SourceChara.Row, bool> ShouldPass;

	public CharaFilter()
	{
		isChara = true;
	}

	protected override bool _ShouldPass(CardRow source)
	{
		if (ShouldPass != null)
		{
			return ShouldPass(source as SourceChara.Row);
		}
		return true;
	}

	public override bool ContainsTag(CardRow source, string str)
	{
		SourceChara.Row row = source as SourceChara.Row;
		if (!source.tag.Contains(str))
		{
			return row.race_row.tag.Contains(str);
		}
		return true;
	}
}
