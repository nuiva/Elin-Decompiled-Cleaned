using System;
using System.Collections.Generic;

public class HomeResource : BaseHomeResource
{
	public override void OnAdvanceDay()
	{
		this.lastValue = this.value;
	}

	public override void Mod(int a, bool popText = true)
	{
		if (a == 0)
		{
			return;
		}
		this.value += a;
		if (popText)
		{
			WidgetPopText.SayValue(base.Name, a, false, base.Sprite);
		}
	}

	public void AddResource(int a, ref string s)
	{
		if (a == 0)
		{
			return;
		}
		this.value += a;
		s = s + (base.Name + " " + a.ToString()).TagColorGoodBad(() => a > 0, false) + ",";
	}

	public class CostList : List<HomeResource.Cost>
	{
		public string GetText()
		{
			string text = "";
			using (List<HomeResource.Cost>.Enumerator enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HomeResource.Cost c = enumerator.Current;
					text = text + (c.resource.Name + ":" + c.cost.ToString()).TagColorGoodBad(() => c.resource.value >= c.cost, () => c.resource.value < c.cost, false) + " ";
				}
			}
			return text;
		}

		public bool CanPay()
		{
			foreach (HomeResource.Cost cost in this)
			{
				if (cost.resource.value < cost.cost)
				{
					return false;
				}
			}
			return true;
		}

		public void Pay()
		{
			foreach (HomeResource.Cost cost in this)
			{
				cost.resource.Mod(-cost.cost, true);
			}
		}
	}

	public class Cost
	{
		public Cost(HomeResource _resource, int _cost)
		{
			this.resource = _resource;
			this.cost = _cost;
		}

		public int cost;

		public HomeResource resource;
	}
}
