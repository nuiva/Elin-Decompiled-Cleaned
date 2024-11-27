using System;

public class ListPeopleBuySlave : BaseListPeople
{
	public SlaverData data
	{
		get
		{
			return this.owner.GetObj<SlaverData>(5);
		}
	}

	public override LayerPeople.ShowMode ShowMode
	{
		get
		{
			return LayerPeople.ShowMode.Work;
		}
	}

	public override bool IsDisabled(Chara c)
	{
		return false;
	}

	public override void OnCreate()
	{
		if (this.data == null)
		{
			this.owner.SetObj(5, new SlaverData());
		}
		this.data.TryRefresh(this.owner);
	}

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		base.OnInstantiate(a, b);
		int money = EClass.pc.GetCurrency("money");
		b.AddPrefab<UIItem>("costMoney").text1.SetText((this.Cost(a).ToString() ?? "").TagColorGoodBad(() => money >= this.Cost(a), false));
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
		foreach (Chara o in this.data.list)
		{
			this.list.Add(o);
		}
	}
}
