public class WidgetTutorial : Widget
{
	public static WidgetTutorial Instance;

	public override void OnActivate()
	{
		Instance = this;
	}
}
