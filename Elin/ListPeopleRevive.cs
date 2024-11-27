using System;
using System.Collections.Generic;
using System.Linq;

public class ListPeopleRevive : BaseListPeople
{
	public override bool IsDisabled(Chara c)
	{
		return false;
	}

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		base.OnInstantiate(a, b);
		int money = EClass.pc.GetCurrency("money");
		b.AddPrefab<UIItem>("costMoney").text1.SetText((CalcMoney.Revive(a).ToString() ?? "").TagColorGoodBad(() => money >= CalcMoney.Revive(a), false));
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (!EClass.pc.TryPay(CalcMoney.Revive(c), "money"))
		{
			return;
		}
		c.GetRevived();
		this.list.List(false);
	}

	public override void OnList()
	{
		foreach (KeyValuePair<int, Chara> keyValuePair in from a in EClass.game.cards.globalCharas
		where a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon
		select a)
		{
			this.list.Add(keyValuePair.Value);
		}
	}
}
