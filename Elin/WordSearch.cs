public class WordSearch : EClass
{
	public string lastWord;

	public void OnValueChanged(string s)
	{
		_ = s == lastWord;
	}

	public void OnSubmit(string s)
	{
		_ = s == lastWord;
	}
}
