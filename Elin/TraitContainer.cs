using System;

public class TraitContainer : TraitBaseContainer
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public CoreRef.InventoryStyle InvStyle
	{
		get
		{
			return EClass.core.refs.invStyle[this.IDInvStyle];
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			this.TrySetAct(p);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (this.owner.c_lockLv > 0)
		{
			p.TrySetAct(new AI_OpenLock
			{
				target = this.owner.Thing
			}, this.owner);
			return;
		}
		if (!this.CanOpenContainer)
		{
			return;
		}
		if (this.owner.GetRootCard() == EClass.pc && this.owner.parentCard != null && !this.owner.parentCard.IsPC && !(this.owner.parentCard.trait is TraitToolBelt))
		{
			return;
		}
		p.TrySetAct("actContainer", delegate()
		{
			this.TryOpen();
			return false;
		}, this.owner, CursorSystem.Container, 1, this.ShowOpenActAsCrime, true, false);
	}

	public void TryOpen()
	{
		if (this.owner.c_lockLv == 0)
		{
			this.Open();
			return;
		}
		if (this.owner.GetRootCard() == EClass.pc && EClass.pc.HasNoGoal && EClass.ui.layers.Count == 0)
		{
			EClass.pc.SetAIImmediate(new AI_OpenLock
			{
				target = this.owner.Thing
			});
			EClass.player.EndTurn(true);
			return;
		}
		SE.Play("lock");
	}

	public virtual void Open()
	{
		if (base.HasChara && !EClass._zone.IsRegion)
		{
			Chara chara = CharaGen.Create(this.owner.c_idRefCard, -1);
			this.owner.c_idRefCard = null;
			EClass._zone.AddCard(chara, (this.owner.ExistsOnMap ? this.owner.pos : EClass.pc.pos).GetNearestPoint(false, false, true, false));
			this.owner.things.DestroyAll(null);
			Msg.Say("package_chara", chara, this.owner, null, null);
			return;
		}
		if (LayerInventory.CreateContainer(this.owner.Thing) != null)
		{
			if (this.InvStyle.sound != null)
			{
				this.InvStyle.sound.Play();
				return;
			}
			SE.PopInventory();
		}
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (LayerInventory.IsOpen(this.owner.Thing))
		{
			b.Attach("open", false);
		}
	}
}
