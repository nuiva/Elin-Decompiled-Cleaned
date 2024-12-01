using Newtonsoft.Json;

public class World : Spatial
{
	[JsonProperty]
	public GameDate date = new GameDate();

	[JsonProperty]
	public Season season = new Season();

	[JsonProperty]
	public Weather weather = new Weather();

	[JsonProperty]
	public Schedule schedule = new Schedule();

	[JsonProperty]
	public DayData dayData;

	[JsonProperty]
	public int ether;

	public Region region => children[0] as Region;

	public override void OnCreate()
	{
		Prologue prologue = EClass.game.Prologue;
		date.year = prologue.year;
		date.month = prologue.month;
		date.day = prologue.day;
		date.hour = prologue.hour;
		for (int i = 0; i < 5; i++)
		{
			schedule.list.Add(new Schedule.Item
			{
				date = date.Copy()
			});
		}
		weather._currentCondition = prologue.weather;
		CreateDayData();
	}

	public void CreateDayData()
	{
		dayData = new DayData();
		int num = EClass.rnd(100);
		for (int i = 0; i < DayData.LuckRange.Length; i++)
		{
			if (num >= DayData.LuckRange[i])
			{
				dayData.luck = i.ToEnum<DayData.Luck>();
				break;
			}
		}
		dayData.seed = EClass.rnd(100000);
	}

	public void ModEther(int a = 3)
	{
		ether += a;
		if (ether >= 100)
		{
			ether = 0;
			weather.SetCondition(Weather.Condition.Ether, 24);
		}
	}

	public void SendPackage(Thing p)
	{
		EClass.game.cards.listPackage.Add(p);
	}
}
