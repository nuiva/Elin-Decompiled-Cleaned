using System;

public class TraitBookShelf : TraitContainer
{
	public override string DefaultIdInvStyle
	{
		get
		{
			return "bookshelf";
		}
	}

	public override string DefaultIdContainer
	{
		get
		{
			return "shop_book";
		}
	}

	public override int DefaultHeight
	{
		get
		{
			return 4;
		}
	}
}
