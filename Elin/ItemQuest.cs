using System;
using UnityEngine.UI;

public class ItemQuest : EMono
{
	public void SetQuest(Quest q)
	{
		this.portrait.SetPerson(q.person);
		this.button.mainText.text = (EMono.game.quests.list.Contains(q) ? "questInProgress".lang().TagColor(FontColor.Good, null) : "") + q.TitlePrefix + q.GetTitle();
		this.button.subText.text = q.person.NameBraced;
		this.textDate.SetText(Date.GetText(q.Hours));
		this.textDate.transform.parent.RebuildLayout(false);
		this.textReward.SetText(q.GetRewardText());
		string text = q.GetDetail(false);
		QuestSupply questSupply = q as QuestSupply;
		if (questSupply != null && questSupply.GetDestThing() != null)
		{
			text = "questSupplyOwned".lang().TagColor(FontColor.Good, null) + text;
		}
		this.textDetail.SetText(text);
		this.imageNew.SetActive(q.isNew);
		q.isNew = false;
		this.textDifficulty.SetText("★".Repeat(q.difficulty));
		this.textDifficulty.SetActive(q.IsRandomQuest);
		string text2 = q.TextExtra;
		this.textExtra.SetActive(!text2.IsEmpty());
		this.textExtra.SetText(text2);
		text2 = q.TextExtra2;
		this.textExtra2.SetActive(!text2.IsEmpty());
		this.textExtra2.SetText(text2);
	}

	public Portrait portrait;

	public UIButton button;

	public UIText textDetail;

	public UIText textReward;

	public UIText textDate;

	public UIText textDifficulty;

	public UIText textExtra;

	public UIText textExtra2;

	public Image imageNew;
}
