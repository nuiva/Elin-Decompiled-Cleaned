using System;
using System.Collections.Generic;
using UnityEngine;

public class InvOwnerMod : InvOwnerDraglet
{
	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.None;
		}
	}

	public override string langTransfer
	{
		get
		{
			return "invMod";
		}
	}

	public static bool IsValidMod(Thing t, SourceElement.Row row)
	{
		return (!(t.trait is TraitToolRangeCane) || row.tag.Contains("cane")) && ((!(t.trait is TraitToolRangeBow) && !(t.trait is TraitToolRangeCrossbow)) || row.id != 601);
	}

	public InvOwnerMod(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency)
	{
		this.count = 1;
	}

	public override bool ShouldShowGuide(Thing t)
	{
		TraitMod traitMod = this.owner.trait as TraitMod;
		if (!InvOwnerMod.IsValidMod(t, traitMod.source))
		{
			return false;
		}
		if (t.sockets != null)
		{
			using (List<int>.Enumerator enumerator = t.sockets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("reloaded");
		EClass.pc.PlayEffect("identify", true, 0f, default(Vector3));
		Msg.Say("modded", t, this.owner, null, null);
		t.ApplySocket(this.owner.Thing);
	}
}
