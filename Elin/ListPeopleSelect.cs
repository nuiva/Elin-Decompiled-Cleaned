using System;
using System.Linq;
using UnityEngine;

public class ListPeopleSelect : BaseListPeople
{
	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		string lang = a.job.GetName().ToTitleCase(true);
		if (this.onShowSubText != null)
		{
			lang = this.onShowSubText(a);
		}
		b.SetSubText2(lang, FontColor.Default, TextAnchor.MiddleRight);
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		this.onClick(c);
		base.layer.Close();
	}

	public override void OnList()
	{
		this.onList(this.list);
	}

	public HireInfo GetInfo(Chara c)
	{
		return EClass.Home.listReserve.First((HireInfo a) => a.chara == c);
	}

	public Action<UIList> onList;

	public Action<Chara> onClick;

	public Func<Chara, string> onShowSubText;
}
