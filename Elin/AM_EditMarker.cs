using System;
using System.Linq;

public class AM_EditMarker : AM_BaseTileSelect
{
	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		foreach (Card card in EClass._map.things.Concat(EClass._map.charas))
		{
			if (!card.c_editorTags.IsEmpty())
			{
				string[] array = card.c_editorTags.Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].ToEnum(true) == EditorTag.Empty)
					{
						card.RemoveThings();
					}
				}
			}
		}
	}

	public override string GetSimpleText(Card c)
	{
		string text = "";
		if (!c.c_idEditor.IsEmpty())
		{
			text = text + "id:" + c.c_idEditor + "\n";
		}
		if (!c.c_idTrait.IsEmpty())
		{
			text = text + "trait:" + c.c_idTrait + "\n";
		}
		if (!c.c_editorTraitVal.IsEmpty())
		{
			text = text + "vals:" + c.c_editorTraitVal + "\n";
		}
		if (!c.c_editorTags.IsEmpty())
		{
			text = text + "tags:" + c.c_editorTags + "\n";
		}
		return text;
	}

	public override HitResult HitTest(Point point, Point start)
	{
		Card target = this.GetTarget(point);
		if (target != null && target.isPlayerCreation)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		Card target = this.GetTarget(point);
		CardInspector.Instance.SetCard(target, true);
	}

	public Card GetTarget(Point point)
	{
		Chara firstChara = point.FirstChara;
		if (firstChara != null)
		{
			return firstChara;
		}
		return point.LastThing;
	}
}
