using System;

public class CharaFilter : CardFilter
{
	public CharaFilter()
	{
		this.isChara = true;
	}

	protected override bool _ShouldPass(CardRow source)
	{
		return this.ShouldPass == null || this.ShouldPass(source as SourceChara.Row);
	}

	public override bool ContainsTag(CardRow source, string str)
	{
		SourceChara.Row row = source as SourceChara.Row;
		return source.tag.Contains(str) || row.race_row.tag.Contains(str);
	}

	public Func<SourceChara.Row, bool> ShouldPass;
}
