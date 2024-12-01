public class ListPeopleBuySlave : BaseListPeople
{
	public SlaverData data => owner.GetObj<SlaverData>(5);

	public override LayerPeople.ShowMode ShowMode => LayerPeople.ShowMode.Work;

	public override bool IsDisabled(Chara c)
	{
		return false;
	}

	public override void OnCreate()
	{
		if (data == null)
		{
			owner.SetObj(5, new SlaverData());
		}
		data.TryRefresh(owner);
	}

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		base.OnInstantiate(a, b);
		int money = EClass.pc.GetCurrency();
		b.AddPrefab<UIItem>("costMoney").text1.SetText((Cost(a).ToString() ?? "").TagColorGoodBad(() => money >= Cost(a)));
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		LayerPeople.slaveToBuy = c;
		base.layer.Close();
	}

	public int Cost(Chara c)
	{
		return CalcMoney.BuySlave(c);
	}

	public override void OnList()
	{
		foreach (Chara item in data.list)
		{
			list.Add(item);
		}
	}
}
