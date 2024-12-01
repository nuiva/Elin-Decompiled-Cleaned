using System;
using System.Linq;

public class ListPeopleSelect : BaseListPeople
{
	public Action<UIList> onList;

	public Action<Chara> onClick;

	public Func<Chara, string> onShowSubText;

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		string lang = a.job.GetName().ToTitleCase(wholeText: true);
		if (onShowSubText != null)
		{
			lang = onShowSubText(a);
		}
		b.SetSubText2(lang);
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		onClick(c);
		base.layer.Close();
	}

	public override void OnList()
	{
		onList(list);
	}

	public HireInfo GetInfo(Chara c)
	{
		return EClass.Home.listReserve.First((HireInfo a) => a.chara == c);
	}
}
