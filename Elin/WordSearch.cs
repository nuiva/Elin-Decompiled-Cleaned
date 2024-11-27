using System;

public class WordSearch : EClass
{
	public void OnValueChanged(string s)
	{
		s == this.lastWord;
	}

	public void OnSubmit(string s)
	{
		s == this.lastWord;
	}

	public string lastWord;
}
