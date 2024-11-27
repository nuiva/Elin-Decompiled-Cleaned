using System;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Season : EClass
{
	public GameDate date
	{
		get
		{
			return EClass.world.date;
		}
	}

	public bool isSpring
	{
		get
		{
			return this.date.month >= 3 && this.date.month <= 5;
		}
	}

	public bool isSummer
	{
		get
		{
			return this.date.month >= 6 && this.date.month <= 8;
		}
	}

	public bool isAutumn
	{
		get
		{
			return this.date.month >= 9 && this.date.month <= 11;
		}
	}

	public bool isWinter
	{
		get
		{
			return this.date.month >= 12 || this.date.month <= 2;
		}
	}

	public Weather.Condition GetRandomWeather(Date date, Weather.Condition current)
	{
		if (EClass.rnd(3) == 0)
		{
			return Weather.Condition.Cloudy;
		}
		if (EClass.rnd(4) == 0)
		{
			return Weather.Condition.Rain;
		}
		if (EClass.rnd(5) == 0)
		{
			return Weather.Condition.RainHeavy;
		}
		if (EClass.rnd(6) == 0)
		{
			return Weather.Condition.Snow;
		}
		return Weather.Condition.Fine;
	}

	public void Next()
	{
		if (this.isSpring)
		{
			this.date.month = 6;
			return;
		}
		if (this.isSummer)
		{
			this.date.month = 9;
			return;
		}
		if (this.isAutumn)
		{
			this.date.month = 12;
			return;
		}
		this.date.month = 3;
	}

	public const int Spring = 1;

	public const int Summer = 2;

	public const int Autumn = 3;

	public const int Winter = 4;
}
