using System;
using UnityEngine;

public class TraitAbility : TraitCatalyst
{
	public override bool CanBeHeldAsFurniture
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool IsRod
	{
		get
		{
			return false;
		}
	}

	public override Act CreateAct()
	{
		return ACT.Create(EClass.sources.elements.alias.TryGetValue(this.owner.c_idAbility ?? "AI_SelfHarm", null));
	}

	public override bool CanAutofire
	{
		get
		{
			return base.act.CanAutofire;
		}
	}

	public override void SetName(ref string s)
	{
		if (this.owner.c_idAbility.IsEmpty())
		{
			return;
		}
		s = EClass.sources.elements.alias[this.owner.c_idAbility].GetName();
	}

	public override void SetMainText(UIText t, bool hotitem)
	{
		Element element = EClass.pc.elements.GetElement(this.owner.c_idAbility);
		if (element == null)
		{
			Debug.Log("[bug] " + this.owner.c_idAbility);
			t.SetActive(false);
			return;
		}
		string text = element.vPotential.ToString() ?? "";
		t.SetText(text ?? "", FontColor.Charge);
		t.SetActive(element is Spell);
	}
}
