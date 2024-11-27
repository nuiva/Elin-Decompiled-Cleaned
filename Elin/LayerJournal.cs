using System;

public class LayerJournal : ELayer
{
	public override bool blockWidgetClick
	{
		get
		{
			return false;
		}
	}

	public override bool HeaderIsListOf(int id)
	{
		return id >= 2 && id <= 4;
	}

	public void SwitchPopulation(int _queryTarget)
	{
		ContentPopulation.queryTarget = _queryTarget;
		ContentPopulation.queryType = 0;
		this.windows[0].SwitchContent<ContentPopulation>();
	}

	public override void OnSwitchContent(Window window)
	{
	}

	public const int IdTabLocation = 2;

	public const int IdTabReligion = 4;
}
