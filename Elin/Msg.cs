using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Msg : EClass
{
	public static WidgetMainText mainText
	{
		get
		{
			return WidgetMainText.Instance;
		}
	}

	public static WidgetFeed feed
	{
		get
		{
			return WidgetFeed.Instance;
		}
	}

	public static MsgColors colors
	{
		get
		{
			return EClass.core.refs.msgColors;
		}
	}

	public static void SetColor()
	{
		Msg.currentColor = Msg.colors.Default;
	}

	public static void SetColor(Color color)
	{
		Msg.currentColor = color;
	}

	public static void SetColor(string id)
	{
		Msg.currentColor = Msg.colors.colors[id];
	}

	public static string GetRawText(string idLang, string ref1, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		Msg.thirdPerson1.Set(ref1);
		Msg.thirdPerson2.Set(ref2);
		return GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(ref1), ref1, ref2, ref3, ref4);
	}

	public static string GetRawText(string idLang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		Msg.thirdPerson1.Set(c1, false);
		Msg.thirdPerson2.Set(c2, false);
		return GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(c1), Msg.GetName(c1), Msg.GetName(c2), ref1, ref2);
	}

	public static string GetRawText(string idLang, Card c1, string ref1 = null, string ref2 = null, string ref3 = null)
	{
		Msg.thirdPerson1.Set(c1, false);
		Msg.thirdPerson2.Set(ref1);
		return GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(c1), Msg.GetName(c1), ref1, ref2, ref3);
	}

	public static string Say(string idLang, string ref1, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		Msg.thirdPerson1.Set(ref1);
		Msg.thirdPerson2.Set(ref2);
		return Msg.SayRaw(GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(ref1), ref1, ref2, ref3, ref4));
	}

	public static string Say(string idLang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		Msg.thirdPerson1.Set(c1, false);
		Msg.thirdPerson2.Set(c2, false);
		return Msg.SayRaw(GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(c1), Msg.GetName(c1), Msg.GetName(c2), ref1, ref2));
	}

	public static string Say(string idLang, Card c1, string ref1 = null, string ref2 = null, string ref3 = null)
	{
		Msg.thirdPerson1.Set(c1, false);
		Msg.thirdPerson2.Set(ref1);
		return Msg.SayRaw(GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(c1), Msg.GetName(c1), ref1, ref2, ref3));
	}

	public static string Say(string idLang, Card c1, int i, string ref1 = null)
	{
		Msg.thirdPerson1.Set(c1, false);
		Msg.thirdPerson2.Set(ref1);
		return Msg.SayRaw(GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(i), Msg.GetName(c1), i.ToString() ?? "", ref1, null));
	}

	public static string Say(string idLang, int i, string ref1 = null, string ref2 = null)
	{
		return Msg.SayRaw(GameLang.Parse(Msg.GetGameText(idLang), Msg.IsThirdPerson(i), ref1, ref2, null, null));
	}

	public static string Say(string idLang)
	{
		return Msg.SayRaw(Msg.GetGameText(idLang));
	}

	public static string SayNothingHappen()
	{
		return Msg.Say("nothingHappens");
	}

	public static string SayCannotUseHere()
	{
		return Msg.Say("cannot_use_here");
	}

	public static void SayGod(string s, Card owner = null)
	{
		Msg.SetColor(Msg.colors.TalkGod);
		Msg.Say(s.Bracket(0));
		if (owner != null)
		{
			owner.SayRaw("@1" + s, null, null);
		}
	}

	public static string SayRaw(string text)
	{
		if (Msg.ignoreAll)
		{
			Msg.currentColor = Msg.colors.Default;
			return "";
		}
		Msg.ToUpperFirst(text);
		if (Msg.feed)
		{
			Msg.feed.System(text);
		}
		if (Msg.mainText)
		{
			Msg.mainText.Append(text, Msg.currentColor, null);
		}
		Msg.currentColor = Msg.colors.Default;
		Msg.alwaysVisible = false;
		EClass.game.log.Add(text, null);
		return text;
	}

	public static void Append(Sprite sprite)
	{
		Msg.mainText.Append(sprite);
	}

	public static void AquireItem(string itemName)
	{
		Msg.Say("getItem", itemName ?? "", null, null, null);
	}

	public static void Nerun(string lang, string idPortrait = "UN_nerun")
	{
		string text = GameLang.Convert(lang.lang());
		if (Msg.feed)
		{
			Msg.feed.Nerun(text, idPortrait);
			return;
		}
		if (Msg.mainText)
		{
			Msg.mainText.Append(text.Bracket(1), Msg.colors.Talk, null);
		}
	}

	public static void SayHomeMember(string lang)
	{
		string text = GameLang.Convert(lang.lang());
		if (Msg.feed)
		{
			Msg.feed.Nerun(text, "UN_nerun");
			return;
		}
		if (Msg.mainText)
		{
			Msg.mainText.Append(text.Bracket(1), Msg.colors.Talk, null);
		}
	}

	public static void SayPic(Card c, string lang)
	{
		if (c == null)
		{
			return;
		}
		string text = GameLang.Convert(lang.lang());
		if (Msg.feed)
		{
			Msg.feed.SayRaw(c, text);
			return;
		}
		if (Msg.mainText)
		{
			Msg.mainText.Append(text.Bracket(1), Msg.colors.Talk, null);
		}
	}

	public static void SayPic(string idPortrait, string lang, string _idPop = null)
	{
		string text = GameLang.Convert(lang.lang());
		if (Msg.feed)
		{
			Msg.feed.SayRaw(idPortrait, text, _idPop);
			return;
		}
		if (Msg.mainText)
		{
			Msg.mainText.Append(text.Bracket(1), Msg.colors.Talk, null);
		}
	}

	public static PopItem Talk(Card c, string id)
	{
		PopItem result = null;
		if (c == null)
		{
			return null;
		}
		string text = GameLang.Convert(c.GetTalkText(id, true, true));
		if (Msg.feed && c != null)
		{
			result = Msg.feed.SayRaw(c, text);
		}
		else if (Msg.mainText)
		{
			Msg.mainText.Append(text.Bracket(1), Msg.colors.Talk, null);
		}
		return result;
	}

	public static PopItem TalkHomeMemeber(string id)
	{
		Chara chara = null;
		if (EClass.Branch != null)
		{
			for (int i = 0; i < 99; i++)
			{
				chara = EClass.Branch.members.RandomItem<Chara>();
				if (chara != EClass.pc)
				{
					break;
				}
			}
		}
		if (chara == null)
		{
			chara = EClass.pc;
		}
		return Msg.Talk(chara, id);
	}

	public static PopItem TalkMaid(string id)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			int uid = chara.uid;
			FactionBranch branch = EClass.Branch;
			int? num = (branch != null) ? new int?(branch.uidMaid) : null;
			if (uid == num.GetValueOrDefault() & num != null)
			{
				return Msg.Talk(chara, id);
			}
		}
		return Msg.TalkHomeMemeber(id);
	}

	public static string GetGameText(string idLang)
	{
		LangGame.Row row = EClass.core.sources.langGame.map.TryGetValue(idLang, null);
		if (row == null)
		{
			return idLang.lang();
		}
		if (!row.effect.IsEmpty() && row.effect == "destroy")
		{
			Msg.mainText.Append(Msg.mainText.spriteDestroy);
		}
		if (!row.color.IsEmpty())
		{
			Msg.SetColor(Msg.colors.colors[row.color]);
		}
		if (!row.sound.IsEmpty())
		{
			EClass.Sound.Play(row.sound);
		}
		return row.GetText("text", false).Split(Environment.NewLine.ToCharArray()).RandomItem<string>();
	}

	public static string GetName(Card c)
	{
		if (c == null)
		{
			return "null";
		}
		if (c.IsPC)
		{
			return "you".lang();
		}
		if (!Msg.alwaysVisible && (EClass.pc.isBlind || !EClass.pc.CanSee(c)) && c.parent == EClass._zone)
		{
			return (c.isChara ? "someone" : "something").lang();
		}
		return c.Name;
	}

	public static bool IsThirdPerson(Card c)
	{
		return c != null && !c.IsPC && c.Num <= 1;
	}

	public static bool IsThirdPerson(string n)
	{
		int i;
		return int.TryParse(n, out i) && Msg.IsThirdPerson(i);
	}

	public static bool IsThirdPerson(int i)
	{
		return i <= 1;
	}

	public static void NewLine()
	{
		if (Msg.mainText)
		{
			Msg.mainText.NewLine();
		}
	}

	public unsafe static void ToUpperFirst(string str)
	{
		if (str == null)
		{
			return;
		}
		fixed (string text = str)
		{
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			*ptr = char.ToUpper(*ptr);
		}
	}

	public static ThirstPersonInfo thirdPerson1 = new ThirstPersonInfo();

	public static ThirstPersonInfo thirdPerson2 = new ThirstPersonInfo();

	public static Color currentColor = Msg.colors.Default;

	public static bool alwaysVisible;

	public static bool ignoreAll;
}
