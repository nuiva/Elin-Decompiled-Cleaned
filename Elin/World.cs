using System;
using Newtonsoft.Json;

public class World : Spatial
{
	public Region region
	{
		get
		{
			return this.children[0] as Region;
		}
	}

	public override void OnCreate()
	{
		Prologue prologue = EClass.game.Prologue;
		this.date.year = prologue.year;
		this.date.month = prologue.month;
		this.date.day = prologue.day;
		this.date.hour = prologue.hour;
		for (int i = 0; i < 5; i++)
		{
			this.schedule.list.Add(new Schedule.Item
			{
				date = this.date.Copy()
			});
		}
		this.weather._currentCondition = prologue.weather;
		this.CreateDayData();
	}

	public void CreateDayData()
	{
		this.dayData = new DayData();
		int num = EClass.rnd(100);
		for (int i = 0; i < DayData.LuckRange.Length; i++)
		{
			if (num >= DayData.LuckRange[i])
			{
				this.dayData.luck = i.ToEnum<DayData.Luck>();
				break;
			}
		}
		this.dayData.seed = EClass.rnd(100000);
	}

	public void ModEther(int a = 3)
	{
		this.ether += a;
		if (this.ether >= 100)
		{
			this.ether = 0;
			this.weather.SetCondition(Weather.Condition.Ether, 24, false);
		}
	}

	public void SendPackage(Thing p)
	{
		EClass.game.cards.listPackage.Add(p);
	}

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
}
