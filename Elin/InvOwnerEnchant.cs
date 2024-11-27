using System;

public class InvOwnerEnchant : InvOwnerEffect
{
	public override bool CanTargetAlly
	{
		get
		{
			return true;
		}
	}

	public override string langTransfer
	{
		get
		{
			return "invEnchant";
		}
	}

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(this.armor ? (this.superior ? 8256 : 8255) : (this.superior ? 8251 : 8250), 1);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.category.IsChildOf(this.armor ? "armor" : "weapon");
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(this.armor ? (this.superior ? EffectId.EnchantArmorGreat : EffectId.EnchantArmor) : (this.superior ? EffectId.EnchantWeaponGreat : EffectId.EnchantWeapon), 100, this.state, t.GetRootCard(), t, default(ActRef));
	}

	public InvOwnerEnchant() : base(null, null, CurrencyType.Money)
	{
	}

	public bool armor;
}
