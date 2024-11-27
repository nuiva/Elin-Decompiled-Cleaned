using System;
using Newtonsoft.Json;

public class Date : EClass
{
	public int year
	{
		get
		{
			return this.raw[0];
		}
		set
		{
			this.raw[0] = value;
		}
	}

	public int month
	{
		get
		{
			return this.raw[1];
		}
		set
		{
			this.raw[1] = value;
		}
	}

	public int day
	{
		get
		{
			return this.raw[2];
		}
		set
		{
			this.raw[2] = value;
		}
	}

	public int hour
	{
		get
		{
			return this.raw[3];
		}
		set
		{
			this.raw[3] = value;
		}
	}

	public int min
	{
		get
		{
			return this.raw[4];
		}
		set
		{
			this.raw[4] = value;
		}
	}

	public int sec
	{
		get
		{
			return this.raw[5];
		}
		set
		{
			this.raw[5] = value;
		}
	}

	public Date Copy()
	{
		return new Date
		{
			year = this.year,
			month = this.month,
			day = this.day,
			hour = this.hour,
			min = this.min,
			sec = this.sec
		};
	}

	public bool IsDay
	{
		get
		{
			return !this.IsNight;
		}
	}

	public bool IsNight
	{
		get
		{
			return this.hour >= 19 || this.hour <= 5;
		}
	}

	public PeriodOfDay periodOfDay
	{
		get
		{
			if (this.hour >= 5 && this.hour <= 6)
			{
				return PeriodOfDay.Dawn;
			}
			if (this.hour >= 7 && this.hour <= 17)
			{
				return PeriodOfDay.Day;
			}
			if (this.hour >= 18 && this.hour <= 19)
			{
				return PeriodOfDay.Dusk;
			}
			return PeriodOfDay.Night;
		}
	}

	public string NameMonth
	{
		get
		{
			return this.month.ToString() ?? "";
		}
	}

	public string NameMonthShort
	{
		get
		{
			return this.month.ToString() ?? "";
		}
	}

	public string NameTime
	{
		get
		{
			return this.periodOfDay.ToString().lang();
		}
	}

	public override string ToString()
	{
		return this.GetText(Date.TextFormat.Log);
	}

	public bool IsSpring
	{
		get
		{
			return this.month >= 3 && this.month <= 5;
		}
	}

	public bool IsSummer
	{
		get
		{
			return this.month >= 6 && this.month <= 8;
		}
	}

	public bool IsAutumn
	{
		get
		{
			return this.month >= 9 && this.month <= 11;
		}
	}

	public bool IsWinter
	{
		get
		{
			return this.month >= 12 || this.month <= 2;
		}
	}

	public void AddHour(int a)
	{
		this.hour += a;
		while (this.hour >= 24)
		{
			this.hour -= 24;
			this.AddDay(1);
		}
	}

	public void AddDay(int a)
	{
		this.day += a;
		while (this.day > 30)
		{
			this.day -= 30;
			this.AddMonth(1);
		}
	}

	public void AddMonth(int a)
	{
		this.month += a;
		while (this.month > 12)
		{
			this.month -= 12;
			int year = this.year;
			this.year = year + 1;
		}
	}

	public string GetText(Date.TextFormat format)
	{
		switch (format)
		{
		case Date.TextFormat.Log:
			return string.Concat(new string[]
			{
				this.month.ToString(),
				"/",
				this.day.ToString(),
				" ",
				this.hour.ToString(),
				":",
				this.min.ToString()
			});
		case Date.TextFormat.Widget:
			return "dateYearMonthDay".lang(this.year.ToString() ?? "", this.month.ToString() ?? "", this.day.ToString() ?? "", null, null);
		case Date.TextFormat.Schedule:
			return "dateSchedule".lang(this.NameMonth, this.day.ToString() ?? "", null, null, null);
		case Date.TextFormat.Travel:
		{
			string str = "_short";
			if (format == Date.TextFormat.Travel)
			{
				return "travelDate".lang(this.year.ToString() ?? "", this.month.ToString() ?? "", this.day.ToString() ?? "", null, null);
			}
			string str2 = "";
			if (this.day != 0)
			{
				str2 = str2 + this.day.ToString() + Lang.Get("wDay" + str);
			}
			if (this.hour != 0)
			{
				str2 = str2 + this.hour.ToString() + Lang.Get("wHour" + str);
			}
			if (this.min != 0)
			{
				str2 = str2 + this.min.ToString() + Lang.Get("wMin" + str);
			}
			return str2 + this.sec.ToString() + Lang.Get("wSec" + str);
		}
		case Date.TextFormat.YearMonthDay:
			return "dateYearMonthDay".lang(this.year.ToString() ?? "", this.month.ToString() ?? "", this.day.ToString() ?? "", null, null);
		case Date.TextFormat.LogPlusYear:
			return string.Concat(new string[]
			{
				this.year.ToString(),
				", ",
				this.month.ToString(),
				"/",
				this.day.ToString(),
				" ",
				this.hour.ToString(),
				":",
				this.min.ToString()
			});
		default:
			return string.Concat(new string[]
			{
				"Day ",
				this.day.ToString(),
				" ",
				this.hour.ToString(),
				":",
				this.min.ToString()
			});
		}
	}

	public static string GetText(int raw, Date.TextFormat format)
	{
		return Date.ToDate(raw).GetText(format);
	}

	public static string GetText(int hour)
	{
		if (hour < 0)
		{
			return "dateDayVoid".lang();
		}
		if (hour > 24)
		{
			return "dateDay".lang((hour / 24).ToString() ?? "", null, null, null, null);
		}
		return "dateHour".lang(hour.ToString() ?? "", null, null, null, null);
	}

	public static string GetText2(int hour)
	{
		if (hour < 0)
		{
			return "-";
		}
		if (hour > 24)
		{
			return (hour / 24).ToString() + "d";
		}
		return hour.ToString() + "h";
	}

	public int GetRawReal(int offsetHours = 0)
	{
		return this.min + (this.hour + offsetHours) * 60 + this.day * 1440 + this.month * 46080 + this.year * 552960;
	}

	public int GetRaw(int offsetHours = 0)
	{
		return this.min + (this.hour + offsetHours) * 60 + this.day * 1440 + this.month * 43200 + this.year * 518400;
	}

	public int GetRawDay()
	{
		return this.day * 1440 + this.month * 43200 + this.year * 518400;
	}

	public bool IsExpired(int time)
	{
		return time - this.GetRaw(0) < 0;
	}

	public int GetRemainingHours(int rawDeadLine)
	{
		return (rawDeadLine - this.GetRaw(0)) / 60;
	}

	public int GetRemainingSecs(int rawDeadLine)
	{
		return rawDeadLine - this.GetRaw(0);
	}

	public int GetElapsedMins(int rawDate)
	{
		return this.GetRaw(0) - rawDate;
	}

	public int GetElapsedHour(int rawDate)
	{
		if (rawDate != 0)
		{
			return (this.GetRaw(0) - rawDate) / 60;
		}
		return 0;
	}

	public static string SecToDate(int sec)
	{
		return string.Concat(new string[]
		{
			(sec / 60 / 60).ToString(),
			"時間 ",
			(sec / 60 % 60).ToString(),
			"分 ",
			(sec % 60).ToString(),
			"秒"
		});
	}

	public static string MinToDayAndHour(int min)
	{
		int num = min / 60;
		int num2 = num / 24;
		if (num == 0)
		{
			return min.ToString() + "分";
		}
		if (num2 != 0)
		{
			return num2.ToString() + "日と" + (num % 24).ToString() + "時間";
		}
		return num.ToString() + "時間";
	}

	public static int[] GetDateArray(int raw)
	{
		return new int[]
		{
			raw % 60,
			raw / 60 % 24,
			raw / 60 / 24 % 30,
			raw / 60 / 24 / 30 % 12,
			raw / 60 / 24 / 30 / 12
		};
	}

	public static Date ToDate(int raw)
	{
		int[] dateArray = Date.GetDateArray(raw);
		int num = dateArray[4];
		int num2 = dateArray[3];
		int num3 = dateArray[2];
		if (num2 == 0)
		{
			num2 = 12;
			num--;
		}
		if (num3 == 0)
		{
			num3 = 30;
			num2--;
		}
		if (num2 == 0)
		{
			num2 = 12;
			num--;
		}
		return new Date
		{
			min = dateArray[0],
			hour = dateArray[1],
			day = num3,
			month = num2,
			year = num
		};
	}

	public const int ShippingHour = 5;

	[JsonProperty]
	public int[] raw = new int[6];

	public const int HourToken = 60;

	public const int DayToken = 1440;

	public const int MonthToken = 43200;

	public const int YearToken = 518400;

	public const int HourTokenReal = 60;

	public const int DayTokenReal = 1440;

	public const int MonthTokenReal = 46080;

	public const int YearTokenReal = 552960;

	public enum TextFormat
	{
		Default,
		Log,
		Widget,
		Schedule,
		Travel,
		YearMonthDay,
		LogPlusYear
	}
}
