using System.IO;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UnityEngine;

public class Steam : MonoBehaviour
{
	public static Steam Instance;

	public SteamworksBehaviour steamworks;

	public UserGeneratedContentQueryManager ugc;

	public WorkshopItem testData;

	public BaseModPackage currentPackage;

	private void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		SteamAPI.Init();
		BaseCore.IsOffline = !App.Client.LoggedOn || SteamSettings.behaviour == null;
	}

	public void CheckUpdate()
	{
	}

	public void CheckDLC()
	{
		Debug.Log(HasDLC(ID_DLC.Test));
	}

	public static bool HasDLC(ID_DLC id)
	{
		if (id != ID_DLC.BackerReward)
		{
			_ = 2504360;
		}
		return EClass.core.config.HasBackerRewardCode();
	}

	public static void GetAchievement(ID_Achievement id)
	{
		string text = "acv_" + id;
		foreach (AchievementObject achievement in Instance.steamworks.settings.achievements)
		{
			if (achievement.Id == text)
			{
				if (!achievement.IsAchieved)
				{
					achievement.Unlock();
					EClass.ui.Say("sys_acv".lang(text.lang()), Resources.Load<Sprite>("Media/Graphics/Icon/Achievement/" + text));
				}
				return;
			}
		}
		Debug.Log("Achievement not found:" + text);
	}

	public void TestHasDLC()
	{
		Debug.Log(HasDLC(ID_DLC.Test));
		Debug.Log(HasDLC(ID_DLC.CursedManor));
	}

	public void CreateUserContent(BaseModPackage p)
	{
		LayerProgress.Start("Uploading").onCancel = delegate
		{
		};
		p.UpdateMeta(updateOnly: true);
		currentPackage = p;
		UgcQuery myPublished = UgcQuery.GetMyPublished();
		myPublished.SetReturnKeyValueTags(tags: true);
		myPublished.Execute(CreateUserContent2);
	}

	private void CreateUserContent2(UgcQuery query)
	{
		Debug.Log("Creating Content2");
		BaseModPackage baseModPackage = currentPackage;
		if (query.ResultsList != null)
		{
			Debug.Log(query.ResultsList.Count);
		}
		foreach (WorkshopItem results in query.ResultsList)
		{
			if (results.keyValueTags == null)
			{
				continue;
			}
			StringKeyValuePair[] keyValueTags = results.keyValueTags;
			for (int i = 0; i < keyValueTags.Length; i++)
			{
				StringKeyValuePair stringKeyValuePair = keyValueTags[i];
				if (stringKeyValuePair.key == "id" && stringKeyValuePair.value == baseModPackage.id && results.Owner.id == App.Client.Owner.id)
				{
					Debug.Log("Updating Content");
					UpdateUserContent(results.FileId);
					return;
				}
			}
		}
		Debug.Log("Creating Content");
		FileInfo fileInfo = new FileInfo(baseModPackage.dirInfo.FullName + "/preview.jpg");
		DirectoryInfo directoryInfo = new DirectoryInfo(baseModPackage.dirInfo.FullName);
		WorkshopItemData workshopItemData = default(WorkshopItemData);
		workshopItemData.appId = steamworks.settings.applicationId;
		workshopItemData.title = baseModPackage.title;
		workshopItemData.description = baseModPackage.description;
		workshopItemData.content = directoryInfo;
		workshopItemData.preview = fileInfo;
		workshopItemData.metadata = baseModPackage.id ?? "";
		workshopItemData.tags = new string[0];
		WorkshopItemData workshopItemData2 = workshopItemData;
		Debug.Log(App.Client.Owner.id);
		Debug.Log(workshopItemData2.appId);
		Debug.Log(baseModPackage.id);
		Debug.Log(directoryInfo.Exists + "/" + directoryInfo.FullName);
		Debug.Log(fileInfo.Exists + "/" + fileInfo.FullName);
		workshopItemData2.Create(null, null, new WorkshopItemKeyValueTag[1]
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
				Dialog.Ok("mod_publish_error");
				Debug.Log("error:" + result.errorMessage);
			}
			else
			{
				Dialog.Ok("mod_created");
				Debug.Log("created");
			}
		});
	}

	public void UpdateUserContent(PublishedFileId_t fileId)
	{
		Debug.Log("Updating Content");
		BaseModPackage baseModPackage = currentPackage;
		WorkshopItemData workshopItemData = default(WorkshopItemData);
		workshopItemData.appId = steamworks.settings.applicationId;
		workshopItemData.title = baseModPackage.title;
		workshopItemData.description = baseModPackage.description;
		workshopItemData.content = new DirectoryInfo(baseModPackage.dirInfo.FullName);
		workshopItemData.preview = new FileInfo(baseModPackage.dirInfo.FullName + "/preview.jpg");
		workshopItemData.metadata = baseModPackage.id ?? "";
		workshopItemData.tags = new string[0];
		WorkshopItemData workshopItemData2 = workshopItemData;
		workshopItemData2.publishedFileId = fileId;
		workshopItemData2.Update(delegate(WorkshopItemDataUpdateStatus result)
		{
			LayerProgress.completed = true;
			if (result.hasError)
			{
				Dialog.Ok("mod_publish_error");
				Debug.Log("error:" + result.errorMessage);
			}
			else
			{
				Dialog.Ok("mod_updated");
				Debug.Log("updated");
			}
		});
	}
}
