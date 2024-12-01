public class LayerJournal : ELayer
{
	public const int IdTabLocation = 2;

	public const int IdTabReligion = 4;

	public override bool blockWidgetClick => false;

	public override bool HeaderIsListOf(int id)
	{
		if (id >= 2)
		{
			return id <= 4;
		}
		return false;
	}

	public void SwitchPopulation(int _queryTarget)
	{
		ContentPopulation.queryTarget = _queryTarget;
		ContentPopulation.queryType = 0;
		windows[0].SwitchContent<ContentPopulation>();
	}

	public override void OnSwitchContent(Window window)
	{
	}
}
