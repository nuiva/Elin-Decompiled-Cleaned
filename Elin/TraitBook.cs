using System;

public class TraitBook : TraitScroll
{
	public bool IsParchment
	{
		get
		{
			return this is TraitParchment;
		}
	}

	public string IdItem
	{
		get
		{
			return this.owner.GetStr(53, null);
		}
	}

	public string IdCat
	{
		get
		{
			if (!this.IsParchment)
			{
				return "Book";
			}
			return "Scroll";
		}
	}

	public BookList.Item Item
	{
		get
		{
			return BookList.GetItem(this.IdItem, this.IdCat);
		}
	}

	public override bool CanStackTo(Thing to)
	{
		return to.GetStr(53, null) == this.IdItem;
	}

	public override void OnCreate(int lv)
	{
		if (base.GetParam(1, null) != null)
		{
			this.owner.SetStr(53, base.GetParam(1, null));
			return;
		}
		if (this.IdItem.IsEmpty())
		{
			this.owner.SetStr(53, BookList.GetRandomItem(this.IdCat).id);
		}
	}

	public override void OnImportMap()
	{
		if (base.GetParam(1, null) != null)
		{
			this.owner.SetStr(53, base.GetParam(1, null));
		}
	}

	public override void SetName(ref string s)
	{
		s = (this.IsParchment ? "_parchment" : "_book").lang(s, this.Item.title, null, null, null);
	}

	public override void OnRead(Chara c)
	{
		BookList.Item item = this.Item;
		EClass.ui.AddLayer<LayerHelp>(this.IsParchment ? "LayerParchment" : "LayerBook").book.Show((this.IsParchment ? "Scroll/" : "Book/") + item.id, null, item.title, item);
	}
}
