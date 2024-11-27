using System;
using System.Collections.Generic;
using UnityEngine;

public class ListPeopleEmbark : ListPeople
{
	public override void OnInstantiate(Chara c, ItemGeneral i)
	{
		i.SetSubText(c.GetActionText(), 240, FontColor.Default, TextAnchor.MiddleLeft);
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		this.charas.Remove(c);
		(this.other as ListPeopleEmbark).charas.Add(c);
		base.MoveToOther(c);
		base.Main.OnRefreshMenu();
	}

	public override void OnList()
	{
		foreach (Chara o in this.charas)
		{
			this.list.Add(o);
		}
	}

	public override void OnRefreshMenu()
	{
		if (!this.main)
		{
			return;
		}
		this.button.interactable = (this.charas.Count > 0 && (this.other as ListPeopleEmbark).charas.Count > 0);
	}

	public List<Chara> charas;

	public UIButton button;
}
