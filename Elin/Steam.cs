using System;
using System.IO;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

public class Steam : MonoBehaviour
{
	private void Awake()
	{
		Steam.Instance = this;
	}

	public void Init()
	{
		SteamAPI.Init();
		BaseCore.IsOffline = (!App.Client.LoggedOn || SteamSettings.behaviour == null);
	}

	public void CheckUpdate()
	{
	}

	public void CheckDLC()
	{
		Debug.Log(Steam.HasDLC(ID_DLC.Test));
	}

	public static bool HasDLC(ID_DLC id)
	{
		if (id != ID_DLC.BackerReward)
		{
		}
		return EClass.core.config.HasBackerRewardCode();
	}

	public static void GetAchievement(ID_Achievement id)
	{
		string text = "acv_" + id.ToString();
		foreach (AchievementObject achievementObject in Steam.Instance.steamworks.settings.achievements)
		{
			if (achievementObject.Id == text)
			{
				if (achievementObject.IsAchieved)
				{
					return;
				}
				achievementObject.Unlock();
				EClass.ui.Say("sys_acv".lang(text.lang(), null, null, null, null), Resources.Load<Sprite>("Media/Graphics/Icon/Achievement/" + text));
				return;
			}
		}
		Debug.Log("Achievement not found:" + text);
	}

	public void TestHasDLC()
	{
		Debug.Log(Steam.HasDLC(ID_DLC.Test));
		Debug.Log(Steam.HasDLC(ID_DLC.CursedManor));
	}

	public void CreateUserContent(BaseModPackage p)
	{
		LayerProgress.Start("Uploading", null).onCancel = delegate()
		{
		};
		p.UpdateMeta(true);
		this.currentPackage = p;
		UgcQuery myPublished = UgcQuery.GetMyPublished();
		myPublished.SetReturnKeyValueTags(true);
		myPublished.Execute(new UnityAction<UgcQuery>(this.CreateUserContent2));
	}

	private void CreateUserContent2(UgcQuery query)
	{
		Debug.Log("Creating Content2");
		BaseModPackage baseModPackage = this.currentPackage;
		if (query.ResultsList != null)
		{
			Debug.Log(query.ResultsList.Count);
		}
		foreach (WorkshopItem workshopItem in query.ResultsList)
		{
			if (workshopItem.keyValueTags != null)
			{
				foreach (StringKeyValuePair stringKeyValuePair in workshopItem.keyValueTags)
				{
					if (stringKeyValuePair.key == "id" && stringKeyValuePair.value == baseModPackage.id && workshopItem.Owner.id == App.Client.Owner.id)
					{
						Debug.Log("Updating Content");
						this.UpdateUserContent(workshopItem.FileId);
						return;
					}
				}
			}
		}
		Debug.Log("Creating Content");
		FileInfo fileInfo = new FileInfo(baseModPackage.dirInfo.FullName + "/preview.jpg");
		DirectoryInfo directoryInfo = new DirectoryInfo(baseModPackage.dirInfo.FullName);
		WorkshopItemData workshopItemData = new WorkshopItemData
		{
			appId = this.steamworks.settings.applicationId,
			title = baseModPackage.title,
			description = baseModPackage.description,
			content = directoryInfo,
			preview = fileInfo,
			metadata = (baseModPackage.id ?? ""),
			tags = new string[0]
		};
		Debug.Log(App.Client.Owner.id);
		Debug.Log(workshopItemData.appId);
		Debug.Log(baseModPackage.id);
		Debug.Log(directoryInfo.Exists.ToString() + "/" + directoryInfo.FullName);
		Debug.Log(fileInfo.Exists.ToString() + "/" + fileInfo.FullName);
		workshopItemData.Create(null, null, new WorkshopItemKeyValueTag[]
		{
			new WorkshopItemKeyValueTag
			{
				key = "id",
				value = baseModPackage.id
			}
		}, delegate(WorkshopItemDataCreateStatus result)
		{
			LayerProgress.completed = true;
			if (result.hasError)
			{
				Dialog.Ok("mod_publish_error", null);
				Debug.Log("error:" + result.errorMessage);
				return;
			}
			Dialog.Ok("mod_created", null);
			Debug.Log("created");
		}, null, null);
	}

	public void UpdateUserContent(PublishedFileId_t fileId)
	{
		Debug.Log("Updating Content");
		BaseModPackage baseModPackage = this.currentPackage;
		WorkshopItemData workshopItemData = new WorkshopItemData
		{
			appId = this.steamworks.settings.applicationId,
			title = baseModPackage.title,
			description = baseModPackage.description,
			content = new DirectoryInfo(baseModPackage.dirInfo.FullName),
			preview = new FileInfo(baseModPackage.dirInfo.FullName + "/preview.jpg"),
			metadata = (baseModPackage.id ?? ""),
			tags = new string[0]
		};
		workshopItemData.publishedFileId = new PublishedFileId_t?(fileId);
		workshopItemData.Update(delegate(WorkshopItemDataUpdateStatus result)
		{
			LayerProgress.completed = true;
			if (result.hasError)
			{
				Dialog.Ok("mod_publish_error", null);
				Debug.Log("error:" + result.errorMessage);
				return;
			}
			Dialog.Ok("mod_updated", null);
			Debug.Log("updated");
		}, null);
	}

	public static Steam Instance;

	public SteamworksBehaviour steamworks;

	public UserGeneratedContentQueryManager ugc;

	public WorkshopItem testData;

	public BaseModPackage currentPackage;
}
