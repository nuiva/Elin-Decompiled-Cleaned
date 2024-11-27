using System;

public class InvOwnerAutoCombatBar : InvOwner
{
	public virtual bool ShowFuel
	{
		get
		{
			return false;
		}
	}

	public virtual string langWhat
	{
		get
		{
			return "";
		}
	}

	public override bool AllowTransfer
	{
		get
		{
			return true;
		}
	}

	public override bool AllowAutouse
	{
		get
		{
			return false;
		}
	}

	public override bool AllowContext
	{
		get
		{
			return false;
		}
	}

	public override bool AllowHold(Thing t)
	{
		return false;
	}

	public override bool UseGuide
	{
		get
		{
			return true;
		}
	}

	public override bool CopyOnTransfer
	{
		get
		{
			return true;
		}
	}

	public InvOwnerAutoCombatBar(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency, PriceType.Default)
	{
	}

	public int index;
}
