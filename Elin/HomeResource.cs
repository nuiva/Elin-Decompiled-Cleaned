using System.Collections.Generic;

public class HomeResource : BaseHomeResource
{
	public class CostList : List<Cost>
	{
		public string GetText()
		{
			string text = "";
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				Cost c = enumerator.Current;
				text = text + (c.resource.Name + ":" + c.cost).TagColorGoodBad(() => c.resource.value >= c.cost, () => c.resource.value < c.cost) + " ";
			}
			return text;
		}

		public bool CanPay()
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Cost current = enumerator.Current;
					if (current.resource.value < current.cost)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void Pay()
		{
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				Cost current = enumerator.Current;
				current.resource.Mod(-current.cost);
			}
		}
	}

	public class Cost
	{
		public int cost;

		public HomeResource resource;

		public Cost(HomeResource _resource, int _cost)
		{
			resource = _resource;
			cost = _cost;
		}
	}

	public override void OnAdvanceDay()
	{
		lastValue = value;
	}

	public override void Mod(int a, bool popText = true)
	{
		if (a != 0)
		{
			value += a;
			if (popText)
			{
				WidgetPopText.SayValue(base.Name, a, negative: false, base.Sprite);
			}
		}
	}

	public void AddResource(int a, ref string s)
	{
		if (a != 0)
		{
			value += a;
			s = s + (base.Name + " " + a).TagColorGoodBad(() => a > 0) + ",";
		}
	}
}
