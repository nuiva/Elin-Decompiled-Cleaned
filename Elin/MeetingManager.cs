using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MeetingManager : EClass
{
	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
		foreach (Meeting meeting in this.list)
		{
			meeting.SetOwner(this.branch);
		}
	}

	public void OnSimulateHour(VirtualDate date)
	{
		if (this.list.Count > 0)
		{
			for (int i = this.list.Count - 1; i >= 0; i--)
			{
				if (this.list[i].dateExipire != 0 && date.IsExpired(this.list[i].dateExipire))
				{
					this.list.RemoveAt(i);
				}
			}
		}
		int count = this.list.Count;
	}

	public void Add(VirtualDate date)
	{
		MeetingMerchant meetingMerchant = new MeetingMerchant();
		meetingMerchant.dateExipire = date.GetRaw(0) + 10080;
		this.list.Add(meetingMerchant);
		if (date.IsRealTime)
		{
			Msg.Say("newMeeting");
		}
	}

	public void Add(Meeting m)
	{
		this.list.Add(m);
	}

	public void Remove(Meeting m)
	{
		this.list.Remove(m);
	}

	public bool CanStartMeeting
	{
		get
		{
			return this.list.Count > 0 && this.SetRoom() != null;
		}
	}

	public BaseArea SetRoom()
	{
		Thing thing = EClass._map.props.installed.Find<TraitSpotMeeting>();
		TraitSpotMeeting traitSpotMeeting = ((thing != null) ? thing.trait : null) as TraitSpotMeeting;
		if (traitSpotMeeting != null)
		{
			this.room = traitSpotMeeting.owner.Cell.room;
			if (this.room == null)
			{
				this.room = new VirtualRoom(traitSpotMeeting.owner);
			}
		}
		else
		{
			this.room = EClass._map.rooms.listRoom.RandomItem<Room>();
		}
		return this.room;
	}

	public void Start()
	{
		this.SetRoom();
		Thing emptySeat = this.room.GetEmptySeat();
		EClass.pc.MoveImmediate(((emptySeat != null) ? emptySeat.pos : null) ?? this.room.GetRandomPoint(true, false), true, true);
		this.CallNext();
	}

	public void CallNext()
	{
		if (this.list.Count == 0)
		{
			return;
		}
		Thing emptySeat = this.room.GetEmptySeat();
		Point chara = ((emptySeat != null) ? emptySeat.pos : null) ?? this.room.GetRandomPoint(true, false);
		Meeting meeting = this.list[0];
		this.list.RemoveAt(0);
		meeting.SetChara(chara);
		Chara maid = EClass.Branch.GetMaid();
		if (maid != null)
		{
			GameLang.refDrama1 = meeting.chara.Name;
			LayerDrama.Activate("_chara", null, "meeting", maid, null, "").SetOnKill(new Action(meeting.Start));
			return;
		}
		meeting.Start();
	}

	[JsonProperty]
	public List<Meeting> list = new List<Meeting>();

	public FactionBranch branch;

	public BaseArea room;
}
