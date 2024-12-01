using System.Collections.Generic;
using Newtonsoft.Json;

public class BaseCard : EClass
{
	[JsonProperty(PropertyName = "X")]
	public Dictionary<int, object> mapObj = new Dictionary<int, object>();

	[JsonProperty(PropertyName = "Y")]
	public Dictionary<int, int> mapInt = new Dictionary<int, int>();

	[JsonProperty(PropertyName = "Z")]
	public Dictionary<int, string> mapStr = new Dictionary<int, string>();

	public bool GetBool(int id)
	{
		return GetInt(id) != 0;
	}

	public void SetBool(int id, bool enable)
	{
		SetInt(id, enable ? 1 : 0);
	}

	public int GetInt(int id, int? defaultInt = null)
	{
		if (mapInt.TryGetValue(id, out var value))
		{
			return value;
		}
		return defaultInt.GetValueOrDefault();
	}

	public void AddInt(int id, int value)
	{
		SetInt(id, GetInt(id) + value);
	}

	public void SetInt(int id, int value = 0)
	{
		if (value == 0)
		{
			if (mapInt.ContainsKey(id))
			{
				mapInt.Remove(id);
			}
		}
		else
		{
			mapInt[id] = value;
		}
	}

	public string GetStr(int id, string defaultStr = null)
	{
		if (mapStr.TryGetValue(id, out var value))
		{
			return value;
		}
		return defaultStr;
	}

	public void SetStr(int id, string value = null)
	{
		if (value.IsEmpty())
		{
			if (mapStr.ContainsKey(id))
			{
				mapStr.Remove(id);
			}
		}
		else
		{
			mapStr[id] = value;
		}
	}

	public T GetObj<T>(int id)
	{
		if (mapObj == null)
		{
			return default(T);
		}
		if (mapObj.TryGetValue(id, out var value) && value is T)
		{
			return (T)value;
		}
		return default(T);
	}

	public void SetObj(int id, object o)
	{
		if (mapObj == null)
		{
			mapObj = new Dictionary<int, object>();
		}
		if (o == null)
		{
			if (mapObj.ContainsKey(id))
			{
				mapObj.Remove(id);
			}
		}
		else
		{
			mapObj[id] = o;
		}
	}

	public T SetObj<T>(int id, object o)
	{
		if (mapObj == null)
		{
			mapObj = new Dictionary<int, object>();
		}
		if (o == null)
		{
			if (mapStr.ContainsKey(id))
			{
				mapObj.Remove(id);
			}
			return default(T);
		}
		mapObj[id] = o;
		return (T)o;
	}
}
