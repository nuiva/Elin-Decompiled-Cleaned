using System.Collections.Generic;

public class ListPeopleEmbark : ListPeople
{
	public List<Chara> charas;

	public UIButton button;

	public override void OnInstantiate(Chara c, ItemGeneral i)
	{
		i.SetSubText(c.GetActionText(), 240);
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		charas.Remove(c);
		(other as ListPeopleEmbark).charas.Add(c);
		MoveToOther(c);
		base.Main.OnRefreshMenu();
	}

	public override void OnList()
	{
		foreach (Chara chara in charas)
		{
			list.Add(chara);
		}
	}

	public override void OnRefreshMenu()
	{
		if (main)
		{
			button.interactable = charas.Count > 0 && (other as ListPeopleEmbark).charas.Count > 0;
		}
	}
}
