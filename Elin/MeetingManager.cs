using System.Collections.Generic;
using Newtonsoft.Json;

public class MeetingManager : EClass
{
	[JsonProperty]
	public List<Meeting> list = new List<Meeting>();

	public FactionBranch branch;

	public BaseArea room;

	public bool CanStartMeeting
	{
		get
		{
			if (list.Count > 0)
			{
				return SetRoom() != null;
			}
			return false;
		}
	}

	public void SetOwner(FactionBranch _branch)
	{
		branch = _branch;
		foreach (Meeting item in list)
		{
			item.SetOwner(branch);
		}
	}

	public void OnSimulateHour(VirtualDate date)
	{
		if (list.Count > 0)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].dateExipire != 0 && date.IsExpired(list[num].dateExipire))
				{
					list.RemoveAt(num);
				}
			}
		}
		_ = list.Count;
	}

	public void Add(VirtualDate date)
	{
		MeetingMerchant meetingMerchant = new MeetingMerchant();
		meetingMerchant.dateExipire = date.GetRaw() + 10080;
		list.Add(meetingMerchant);
		if (date.IsRealTime)
		{
			Msg.Say("newMeeting");
		}
	}

	public void Add(Meeting m)
	{
		list.Add(m);
	}

	public void Remove(Meeting m)
	{
		list.Remove(m);
	}

	public BaseArea SetRoom()
	{
		if (EClass._map.props.installed.Find<TraitSpotMeeting>()?.trait is TraitSpotMeeting traitSpotMeeting)
		{
			room = traitSpotMeeting.owner.Cell.room;
			if (room == null)
			{
				room = new VirtualRoom(traitSpotMeeting.owner);
			}
		}
		else
		{
			room = EClass._map.rooms.listRoom.RandomItem();
		}
		return room;
	}

	public void Start()
	{
		SetRoom();
		Thing emptySeat = room.GetEmptySeat();
		EClass.pc.MoveImmediate(emptySeat?.pos ?? room.GetRandomPoint(walkable: true, allowChara: false));
		CallNext();
	}

	public void CallNext()
	{
		if (list.Count != 0)
		{
			Point chara = room.GetEmptySeat()?.pos ?? room.GetRandomPoint(walkable: true, allowChara: false);
			Meeting meeting = list[0];
			list.RemoveAt(0);
			meeting.SetChara(chara);
			Chara maid = EClass.Branch.GetMaid();
			if (maid != null)
			{
				GameLang.refDrama1 = meeting.chara.Name;
				LayerDrama.Activate("_chara", null, "meeting", maid).SetOnKill(meeting.Start);
			}
			else
			{
				meeting.Start();
			}
		}
	}
}
