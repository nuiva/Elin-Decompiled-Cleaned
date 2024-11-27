using System;

public class BackerContent : EClass
{
	public static string ConvertName(string s)
	{
		return s.Replace("『", "「");
	}

	public static void GakiConvert(ref string text, string idLang = "zako")
	{
		if (!text.StartsWith("("))
		{
			text = idLang.lang().Split(',', StringSplitOptions.None).RandomItem<string>() + " (" + text + ")";
		}
		else if (idLang == "mokyu")
		{
			text = idLang.lang().Split(',', StringSplitOptions.None).RandomItem<string>() + " " + text;
		}
		text = text.Replace("。)", ")");
		text = text.Replace("」", "");
	}

	public static int indexTree;

	public static int indexRemain;

	public static int indexLantern;

	public static int indexSnail;

	public static int indexFollower;

	public static int indexSister;
}
