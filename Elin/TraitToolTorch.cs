using System;

public class TraitToolTorch : TraitTool
{
	public bool IsLit
	{
		get
		{
			return EClass.pc.GetCondition<ConTorch>() != null;
		}
	}

	public override void OnEnterScreen()
	{
		this.RefreshRenderer();
	}

	public override void RefreshRenderer()
	{
		if (!this.owner.renderer.isSynced)
		{
			return;
		}
		if (this.IsLit)
		{
			this.owner.renderer.AddExtra("torch_held");
			return;
		}
		this.owner.renderer.RemoveExtra("torch_held");
	}

	public override void OnSetCurrentItem()
	{
		EClass.pc.RecalculateFOV();
		if (this.IsLit)
		{
			EClass.pc.PlaySound("torch_lit", 1f, true);
		}
	}

	public override void OnUnsetCurrentItem()
	{
		EClass.pc.RecalculateFOV();
		this.RefreshRenderer();
	}

	public void ToggleOn()
	{
		EClass.pc.Say("torch_start", EClass.pc, this.owner, null, null);
		EClass.pc.AddCondition<ConTorch>(100, false);
		EClass.pc.PlaySound("torch_lit", 1f, true);
		this.RefreshRenderer();
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		ConTorch con = EClass.pc.GetCondition<ConTorch>();
		if (p.IsSelfOrNeighbor)
		{
			Func<bool> <>9__1;
			foreach (Card card in p.pos.ListCards(false))
			{
				if (card.trait.IsLighting && con == null && card.isOn)
				{
					string lang = "ActTorch";
					Func<bool> onPerform;
					if ((onPerform = <>9__1) == null)
					{
						onPerform = (<>9__1 = delegate()
						{
							this.ToggleOn();
							return true;
						});
					}
					p.TrySetAct(lang, onPerform, this.owner, null, 1, false, true, false);
					break;
				}
			}
		}
		if (p.IsSelf)
		{
			if (con != null)
			{
				p.TrySetAct("ActExtinguishTorch", delegate()
				{
					con.Kill(false);
					this.RefreshRenderer();
					return true;
				}, null, 1);
				return;
			}
			Thing log = EClass.pc.things.Find("log", -1, -1);
			string text = "ActTorch".lang() + " ";
			string str = text;
			string s = "consumeResource";
			string name = EClass.sources.cards.map["log"].GetName();
			string @ref = 1.ToString() ?? "";
			Thing log2 = log;
			text = str + s.lang(name, @ref, ((log2 != null) ? log2.Num : 0).ToString() ?? "", null, null);
			p.TrySetAct(text, delegate()
			{
				if (log == null)
				{
					Msg.Say("noLogForTorch");
					return false;
				}
				log.ModNum(-1, true);
				this.ToggleOn();
				return true;
			}, this.owner, null, 1, false, true, false);
		}
	}
}
