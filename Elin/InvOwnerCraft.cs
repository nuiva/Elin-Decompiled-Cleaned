using System;

public class InvOwnerCraft : InvOwnerDraglet
{
	public override bool ShowFuel
	{
		get
		{
			return this.crafter.IsRequireFuel;
		}
	}

	public override string langTransfer
	{
		get
		{
			return this.crafter.CrafterTitle;
		}
	}

	public override int numDragGrid
	{
		get
		{
			return this.crafter.numIng;
		}
	}

	public override bool DenyImportant
	{
		get
		{
			return false;
		}
	}

	public override bool AllowStockIngredients
	{
		get
		{
			return EClass._zone.IsPCFaction || EClass._zone is Zone_Tent;
		}
	}

	public InvOwnerCraft(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return this.crafter.IsCraftIngredient(t, base.dragGrid.currentIndex);
	}

	public override void _OnProcess(Thing t)
	{
		t.PlaySoundDrop(false);
		this.TryStartCraft();
	}

	public override void OnAfterRefuel()
	{
		this.TryStartCraft();
	}

	public void TryStartCraft()
	{
		for (int i = 0; i < this.numDragGrid; i++)
		{
			if (base.dragGrid.buttons[i].Card == null)
			{
				return;
			}
		}
		if (!this.owner.trait.IsFuelEnough(1, base.dragGrid.GetTargets(), true))
		{
			Msg.Say("notEnoughFuel");
			return;
		}
		EClass.pc.SetAI(new AI_UseCrafter
		{
			crafter = this.crafter,
			layer = base.dragGrid
		});
	}

	public TraitCrafter crafter;
}
