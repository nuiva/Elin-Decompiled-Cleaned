using System;

public class ThingFilter : CardFilter
{
	protected override bool _ShouldPass(CardRow source)
	{
		return this.ShouldPass == null || this.ShouldPass(source as SourceThing.Row);
	}

	public Func<SourceThing.Row, bool> ShouldPass;
}
