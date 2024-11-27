using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BaseCard : EClass
{
	public bool GetBool(int id)
	{
		return this.GetInt(id, null) != 0;
	}

	public void SetBool(int id, bool enable)
	{
		this.SetInt(id, enable ? 1 : 0);
	}

	public int GetInt(int id, int? defaultInt = null)
	{
		int result;
		if (this.mapInt.TryGetValue(id, out result))
		{
			return result;
		}
		return defaultInt.GetValueOrDefault();
	}

	public void AddInt(int id, int value)
	{
		this.SetInt(id, this.GetInt(id, null) + value);
	}

	public void SetInt(int id, int value = 0)
	{
		if (value == 0)
		{
			if (this.mapInt.ContainsKey(id))
			{
				this.mapInt.Remove(id);
			}
			return;
		}
		this.mapInt[id] = value;
	}

	public string GetStr(int id, string defaultStr = null)
	{
		string result;
		if (this.mapStr.TryGetValue(id, out result))
		{
			return result;
		}
		return defaultStr;
	}

	public void SetStr(int id, string value = null)
	{
		if (value.IsEmpty())
		{
			if (this.mapStr.ContainsKey(id))
			{
				this.mapStr.Remove(id);
			}
			return;
		}
		this.mapStr[id] = value;
	}

	public T GetObj<T>(int id)
	{
		if (this.mapObj == null)
		{
			return default(T);
		}
		object obj;
		if (this.mapObj.TryGetValue(id, out obj) && obj is T)
		{
			return (T)((object)obj);
		}
		return default(T);
	}

	public void SetObj(int id, object o)
	{
		if (this.mapObj == null)
		{
			this.mapObj = new Dictionary<int, object>();
		}
		if (o == null)
		{
			if (this.mapObj.ContainsKey(id))
			{
				this.mapObj.Remove(id);
			}
			return;
		}
		this.mapObj[id] = o;
	}

	public T SetObj<T>(int id, object o)
	{
		if (this.mapObj == null)
		{
			this.mapObj = new Dictionary<int, object>();
		}
		if (o == null)
		{
			if (this.mapStr.ContainsKey(id))
			{
				this.mapObj.Remove(id);
			}
			return default(T);
		}
		this.mapObj[id] = o;
		return (T)((object)o);
	}

	[JsonProperty(PropertyName = "X")]
	public Dictionary<int, object> mapObj = new Dictionary<int, object>();

	[JsonProperty(PropertyName = "Y")]
	public Dictionary<int, int> mapInt = new Dictionary<int, int>();

	[JsonProperty(PropertyName = "Z")]
	public Dictionary<int, string> mapStr = new Dictionary<int, string>();
}
