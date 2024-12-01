public class TraitToolTorch : TraitTool
{
	public bool IsLit => EClass.pc.GetCondition<ConTorch>() != null;

	public override void OnEnterScreen()
	{
		RefreshRenderer();
	}

	public override void RefreshRenderer()
	{
		if (owner.renderer.isSynced)
		{
			if (IsLit)
			{
				owner.renderer.AddExtra("torch_held");
			}
			else
			{
				owner.renderer.RemoveExtra("torch_held");
			}
		}
	}

	public override void OnSetCurrentItem()
	{
		EClass.pc.RecalculateFOV();
		if (IsLit)
		{
			EClass.pc.PlaySound("torch_lit");
		}
	}

	public override void OnUnsetCurrentItem()
	{
		EClass.pc.RecalculateFOV();
		RefreshRenderer();
	}

	public void ToggleOn()
	{
		EClass.pc.Say("torch_start", EClass.pc, owner);
		EClass.pc.AddCondition<ConTorch>();
		EClass.pc.PlaySound("torch_lit");
		RefreshRenderer();
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		ConTorch con = EClass.pc.GetCondition<ConTorch>();
		if (p.IsSelfOrNeighbor)
		{
			foreach (Card item in p.pos.ListCards())
			{
				if (item.trait.IsLighting && con == null && item.isOn)
				{
					p.TrySetAct("ActTorch", delegate
					{
						ToggleOn();
						return true;
					}, owner);
					break;
				}
			}
		}
		if (!p.IsSelf)
		{
			return;
		}
		if (con != null)
		{
			p.TrySetAct("ActExtinguishTorch", delegate
			{
				con.Kill();
				RefreshRenderer();
				return true;
			});
			return;
		}
		Thing log = EClass.pc.things.Find("log");
		string text = "ActTorch".lang() + " ";
		text += "consumeResource".lang(EClass.sources.cards.map["log"].GetName(), 1.ToString() ?? "", (log?.Num ?? 0).ToString() ?? "");
		p.TrySetAct(text, delegate
		{
			if (log == null)
			{
				Msg.Say("noLogForTorch");
				return false;
			}
			log.ModNum(-1);
			ToggleOn();
			return true;
		}, owner);
	}
}
