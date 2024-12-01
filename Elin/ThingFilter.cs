using System;

public class ThingFilter : CardFilter
{
	public Func<SourceThing.Row, bool> ShouldPass;

	protected override bool _ShouldPass(CardRow source)
	{
		if (ShouldPass != null)
		{
			return ShouldPass(source as SourceThing.Row);
		}
		return true;
	}
}
