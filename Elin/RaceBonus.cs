using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RaceBonus : Element
{
	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_Feat");
	}

	public override bool ShowValue
	{
		get
		{
			return false;
		}
	}

	public override void OnWriteNote(UINote n, ElementContainer owner)
	{
		RaceBonus.<>c__DisplayClass3_0 CS$<>8__locals1;
		CS$<>8__locals1.owner = owner;
		CS$<>8__locals1.n = n;
		RaceBonus.<OnWriteNote>g__AddText|3_0(60, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(61, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(79, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(65, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(64, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(55, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(56, ref CS$<>8__locals1);
		RaceBonus.<OnWriteNote>g__AddText|3_0(57, ref CS$<>8__locals1);
	}

	[CompilerGenerated]
	internal static void <OnWriteNote>g__AddText|3_0(int e, ref RaceBonus.<>c__DisplayClass3_0 A_1)
	{
		int num = A_1.owner.Card.Evalue(e);
		if (num != 0)
		{
			A_1.n.AddTopic("TopicLeft", EClass.sources.elements.map[e].GetName(), num.ToString() ?? "");
		}
	}
}
