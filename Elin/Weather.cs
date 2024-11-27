using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class Weather : EClass
{
	public Weather.Condition CurrentCondition
	{
		get
		{
			if (EClass._map.config.fixedCondition == Weather.Condition.None)
			{
				return this._currentCondition;
			}
			return EClass._map.config.fixedCondition;
		}
	}

	public Season season
	{
		get
		{
			return EClass.world.season;
		}
	}

	public bool IsHazard
	{
		get
		{
			return this.CurrentCondition == Weather.Condition.Rain || this.CurrentCondition == Weather.Condition.RainHeavy || this.CurrentCondition == Weather.Condition.Snow || this.CurrentCondition == Weather.Condition.SnowHeavy || this.CurrentCondition == Weather.Condition.Ether;
		}
	}

	public bool IsFineOrCloudy
	{
		get
		{
			return this.CurrentCondition == Weather.Condition.Fine || this.CurrentCondition == Weather.Condition.Cloudy;
		}
	}

	public bool IsRaining
	{
		get
		{
			return this.CurrentCondition == Weather.Condition.Rain || this.CurrentCondition == Weather.Condition.RainHeavy;
		}
	}

	public bool IsSnowing
	{
		get
		{
			return this.CurrentCondition == Weather.Condition.Snow || this.CurrentCondition == Weather.Condition.SnowHeavy;
		}
	}

	public bool IsBlossom
	{
		get
		{
			return this.CurrentCondition == Weather.Condition.Blossom || (this.IsFineOrCloudy && EClass._map.config.blossom);
		}
	}

	public bool IsEther
	{
		get
		{
			return this.CurrentCondition == Weather.Condition.Ether;
		}
	}

	public string GetName()
	{
		return this.GetName(this.CurrentCondition);
	}

	public string GetName(Weather.Condition condition)
	{
		return ("weather" + condition.ToString()).lang();
	}

	public void RefreshWeather()
	{
		if (EClass._map.IsIndoor)
		{
			return;
		}
		bool flag = EClass._zone.IsSnowCovered;
		if (EClass._zone is Region)
		{
			EloMap elomap = EClass.scene.elomapActor.elomap;
			EloMap.TileInfo tileInfo = elomap.GetTileInfo(EClass.pc.pos.x + elomap.minX, EClass.pc.pos.z + elomap.minY);
			flag = (tileInfo != null && tileInfo.IsSnow);
			if (EClass.world.season.isWinter)
			{
				flag = true;
			}
		}
		if (this.IsSnowing)
		{
			if (!flag)
			{
				this._currentCondition = ((this._currentCondition == Weather.Condition.Snow) ? Weather.Condition.Rain : Weather.Condition.RainHeavy);
				return;
			}
		}
		else if (this.IsRaining && flag)
		{
			this._currentCondition = ((this._currentCondition == Weather.Condition.Rain) ? Weather.Condition.Snow : Weather.Condition.SnowHeavy);
		}
	}

	public void OnChangeHour()
	{
		this.duration--;
		if (this.duration < 0)
		{
			this.SetConditionFromForecast(false);
		}
	}

	public Weather.Forecast GetForecast(Date date, Weather.Condition current)
	{
		Weather.Forecast forecast = new Weather.Forecast();
		forecast.condition = ((EClass.rnd(4) != 0) ? Weather.Condition.Fine : this.season.GetRandomWeather(date, current));
		forecast.duration = EClass.rnd(24) + 10;
		date.AddHour(forecast.duration);
		return forecast;
	}

	public List<Weather.WeatherForecast> GetWeatherForecast()
	{
		this.RefreshForecasts();
		List<Weather.WeatherForecast> list = new List<Weather.WeatherForecast>();
		Date date = EClass.world.date.Copy();
		Weather.WeatherForecast weatherForecast = new Weather.WeatherForecast
		{
			date = date.Copy()
		};
		list.Add(weatherForecast);
		Weather.Forecast forecast = new Weather.Forecast
		{
			condition = this._currentCondition,
			duration = this.duration
		};
		int num = forecast.duration;
		int num2 = -1;
		for (int i = 0; i < 10000; i++)
		{
			num--;
			weatherForecast.Add(forecast.condition, 1);
			if (num < 0)
			{
				num2++;
				if (this.forecasts.Count <= num2)
				{
					list.Remove(weatherForecast);
					break;
				}
				forecast = this.forecasts[num2];
				num = forecast.duration;
			}
			date.AddHour(1);
			if (date.day != weatherForecast.date.day)
			{
				weatherForecast.Finish();
				weatherForecast = new Weather.WeatherForecast
				{
					date = date.Copy()
				};
				list.Add(weatherForecast);
			}
		}
		return list;
	}

	public void RefreshForecasts()
	{
		if (this.forecasts.Count < 10)
		{
			Date date = EClass.world.date.Copy();
			Weather.Condition current = Weather.Condition.Fine;
			for (int i = 0; i < 10; i++)
			{
				if (this.forecasts.Count > i)
				{
					Weather.Forecast forecast = this.forecasts[i];
					date.AddHour(forecast.duration);
					current = forecast.condition;
				}
				else
				{
					Weather.Forecast forecast2 = this.GetForecast(date, current);
					this.forecasts.Add(forecast2);
					current = forecast2.condition;
				}
			}
		}
	}

	public long GetTimeSinceLastRain()
	{
		if (this.CurrentCondition == Weather.Condition.Rain || this.CurrentCondition == Weather.Condition.RainHeavy)
		{
			return 0L;
		}
		return (long)EClass.world.date.GetElapsedMins(this.lastRain);
	}

	public void SetRandomCondition()
	{
		this.SetCondition(Util.RandomEnum<Weather.Condition>(), 20, false);
	}

	public void SetConditionFromForecast(bool silent = false)
	{
		this.RefreshForecasts();
		Weather.Forecast forecast = this.forecasts[0];
		this.SetCondition(forecast.condition, forecast.duration, silent);
		this.forecasts.RemoveAt(0);
	}

	public void SetCondition(Weather.Condition condition, int _duration = 20, bool silent = false)
	{
		if (condition == Weather.Condition.Fine)
		{
			foreach (Chara chara in EClass.pc.party.members)
			{
				if (chara.HasElement(1558, 1) && EClass.rnd(4) == 0)
				{
					condition = Weather.Condition.Rain;
					Msg.Say("drawRain", chara, null, null, null);
					break;
				}
			}
		}
		if (EClass.world.season.isWinter)
		{
			if (condition == Weather.Condition.Rain)
			{
				condition = Weather.Condition.Snow;
			}
			else if (condition == Weather.Condition.RainHeavy)
			{
				condition = Weather.Condition.SnowHeavy;
			}
		}
		else if (!EClass._zone.IsSnowZone && !EClass._map.IsIndoor)
		{
			if (condition == Weather.Condition.Snow)
			{
				condition = Weather.Condition.Rain;
			}
			else if (condition == Weather.Condition.SnowHeavy)
			{
				condition = Weather.Condition.RainHeavy;
			}
		}
		if (this._currentCondition == Weather.Condition.Rain || this._currentCondition == Weather.Condition.RainHeavy)
		{
			this.lastRain = EClass.world.date.GetRaw(0);
		}
		this.duration = _duration;
		if (condition == this._currentCondition)
		{
			return;
		}
		this._currentCondition = condition;
		this.RefreshWeather();
		Msg.Say("weather_" + condition.ToString());
		if (this.IsRaining)
		{
			EClass._zone.RainWater();
		}
		EClass.core.screen.RefreshSky();
	}

	private const int maxForecasts = 10;

	[JsonProperty]
	public Weather.Condition _currentCondition;

	[JsonProperty]
	public int duration = 8;

	[JsonProperty]
	public int lastRain;

	[JsonProperty]
	public List<Weather.Forecast> forecasts = new List<Weather.Forecast>();

	[JsonObject(MemberSerialization.OptOut)]
	public class Forecast
	{
		public int duration;

		public Weather.Condition condition;
	}

	public enum Condition
	{
		Fine,
		Cloudy,
		Rain,
		RainHeavy,
		Snow,
		SnowHeavy,
		Ether,
		Blossom,
		None
	}

	public class WeatherForecast
	{
		public void Add(Weather.Condition con, int h)
		{
			if (this.cons.ContainsKey(con))
			{
				Dictionary<Weather.Condition, int> dictionary = this.cons;
				dictionary[con] += h;
				return;
			}
			this.cons[con] = h;
		}

		public void Finish()
		{
			if (this.cons.Count == 0)
			{
				return;
			}
			int num = this.cons.Sum((KeyValuePair<Weather.Condition, int> c) => c.Value);
			if (num != 0)
			{
				foreach (Weather.Condition key in this.cons.Keys.ToList<Weather.Condition>())
				{
					this.cons[key] = this.cons[key] * 100 / num;
				}
			}
		}

		public Date date;

		public Dictionary<Weather.Condition, int> cons = new Dictionary<Weather.Condition, int>();
	}
}
