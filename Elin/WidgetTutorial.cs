using System;

public class WidgetTutorial : Widget
{
	public override void OnActivate()
	{
		WidgetTutorial.Instance = this;
	}

	public static WidgetTutorial Instance;
}
