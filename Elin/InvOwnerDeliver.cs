using System;

public class InvOwnerDeliver : InvOwnerDraglet
{
	public override bool SingleTarget
	{
		get
		{
			return this.mode == InvOwnerDeliver.Mode.Tax;
		}
	}

	public override string langTransfer
	{
		get
		{
			if (this.mode != InvOwnerDeliver.Mode.Bank)
			{
				return "invDeliver";
			}
			return "invBank";
		}
	}

	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.Consume;
		}
	}

	public InvOwnerDeliver(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		switch (this.mode)
		{
		case InvOwnerDeliver.Mode.Tax:
			return t.c_bill != 0;
		case InvOwnerDeliver.Mode.Bank:
			return t.id == "money";
		case InvOwnerDeliver.Mode.Crop:
			return t.category.id == "vegi" || t.category.id == "fruit" || t.category.id == "mushroom";
		default:
			return false;
		}
	}

	public static void PayBill(Thing t, bool fromBank = false)
	{
		bool flag = t.id == "bill_tax";
		bool flag2 = t.id == "bill_debt";
		QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
		bool flag3 = flag2 && questDebt != null && !questDebt.paid && questDebt.IsValidBill(t);
		if ((flag && EClass.player.taxBills <= 0) || (flag2 && !flag3))
		{
			SE.Beep();
			Msg.Say("badidea");
			EClass.pc.Pick(t, false, true);
			return;
		}
		if (fromBank)
		{
			EClass.game.cards.container_deposit.ModCurrency(-t.c_bill, "money");
		}
		else if (!EClass.pc.TryPay(t.c_bill, "money"))
		{
			return;
		}
		if (flag2)
		{
			EClass.player.debt -= t.c_bill;
			if (EClass.player.debt < 0)
			{
				EClass.player.debt = 0;
			}
			questDebt.paid = true;
			questDebt.UpdateJournal();
		}
		else if (flag)
		{
			EClass.player.stats.taxBillsPaid += t.c_bill;
			EClass.player.taxBills--;
			if (EClass.player.taxBills < 0)
			{
				EClass.player.taxBills = 0;
			}
			int num = t.GetInt(35, null) / 1000;
			if (num > 0)
			{
				Thing thing = ThingGen.Create("money2", "copper").SetNum(num);
				Thing p = ThingGen.CreateParcel("parcel_mysiliaGift", new Thing[]
				{
					thing
				});
				Msg.Say("getSalary", thing, null, null, null);
				EClass.world.SendPackage(p);
			}
		}
		else
		{
			EClass.player.unpaidBill -= t.c_bill;
		}
		Msg.Say("payBill", t, null, null, null);
		t.Destroy();
	}

	public override void _OnProcess(Thing t)
	{
		switch (this.mode)
		{
		case InvOwnerDeliver.Mode.Tax:
			InvOwnerDeliver.PayBill(t, false);
			return;
		case InvOwnerDeliver.Mode.Bank:
			SE.Pay();
			Msg.Say("depositMoney", t, this.owner, null, null);
			this.owner.AddThing(t, true, -1, -1);
			return;
		case InvOwnerDeliver.Mode.Crop:
		{
			Msg.Say("farm_chest", t, Lang._weight(t.SelfWeight * t.Num, true, 0), null, null);
			QuestHarvest questHarvest = EClass.game.quests.Get<QuestHarvest>();
			if (questHarvest != null)
			{
				questHarvest.weightDelivered += t.SelfWeight * t.Num;
			}
			SE.Pick();
			t.Destroy();
			return;
		}
		default:
			return;
		}
	}

	public InvOwnerDeliver.Mode mode;

	public enum Mode
	{
		Default,
		Tax,
		Bank,
		Crop
	}
}
