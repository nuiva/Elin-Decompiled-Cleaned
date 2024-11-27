using System;

public class TraitMaterialHammer : TraitItem
{
	public override bool OnUse(Chara c)
	{
		ActEffect.Proc(EffectId.ChangeMaterial, EClass.pc, null, 100, new ActRef
		{
			n1 = this.owner.material.alias,
			refThing = this.owner.Thing
		});
		return true;
	}

	public override int GetValue()
	{
		return 5000 + this.owner.sourceCard.value * this.owner.material.value * this.owner.material.value / 1000;
	}
}
