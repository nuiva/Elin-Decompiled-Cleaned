public class TraitMaterialHammer : TraitItem
{
	public override bool OnUse(Chara c)
	{
		ActEffect.Proc(EffectId.ChangeMaterial, EClass.pc, null, 100, new ActRef
		{
			n1 = owner.material.alias,
			refThing = owner.Thing
		});
		return true;
	}

	public override int GetValue()
	{
		return 5000 + owner.sourceCard.value * owner.material.value * owner.material.value / 1000;
	}
}
