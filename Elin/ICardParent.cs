public interface ICardParent
{
	ICardParent GetRoot();

	void RemoveCard(Card c);

	void OnChildNumChange(Card c);
}
