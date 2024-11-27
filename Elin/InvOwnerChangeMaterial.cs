using System;

public class InvOwnerChangeMaterial : InvOwnerEffect
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
			return "invChangeMaterial";
		}
	}

	public override string langWhat
	{
		get
		{
			return "changeMaterial_what";
		}
	}

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.Create("mathammer", this.mat.alias);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.trait is TraitGodStatue || (!t.category.IsChildOf("currency") && !t.IsUnique && t.trait.CanBeDropped && !t.source.fixedMaterial && !(t.trait is TraitCatalyst) && !(t.trait is TraitTile) && !(t.trait is TraitMaterialHammer) && !(t.trait is TraitSeed));
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(this.idEffect, 100, this.state, t.GetRootCard(), t, new ActRef
		{
			n1 = this.mat.alias
		});
		if (this.consume != null)
		{
			this.consume.ModNum(-1, true);
		}
		TraitGodStatue traitGodStatue = t.trait as TraitGodStatue;
		if (traitGodStatue != null)
		{
			traitGodStatue.OnChangeMaterial();
		}
	}

	public InvOwnerChangeMaterial() : base(null, null, CurrencyType.Money)
	{
	}

	public SourceMaterial.Row mat;

	public Thing consume;
}
