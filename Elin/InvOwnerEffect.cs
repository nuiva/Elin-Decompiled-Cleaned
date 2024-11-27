using System;

public class InvOwnerEffect : InvOwnerDraglet
{
	public InvOwnerEffect(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money) : base(owner, container, _currency)
	{
	}

	public Chara cc;

	public EffectId idEffect;

	public bool superior;
}
