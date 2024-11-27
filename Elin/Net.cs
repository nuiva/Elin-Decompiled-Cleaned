using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Net : MonoBehaviour
{
	public static bool ShowNetError
	{
		get
		{
			return EClass.core.config.test.showNetError || EClass.debug.enable;
		}
	}

	public void ShowVote(string logs)
	{
		StringReader stringReader = new StringReader(logs);
		for (string message = stringReader.ReadLine(); message != null; message = stringReader.ReadLine())
		{
			Debug.Log(message);
		}
	}

	public void ShowChat(string logs)
	{
		foreach (JToken jtoken in ((JArray)JsonConvert.DeserializeObject(logs)))
		{
			Net.ChatLog chatLog = jtoken.ToObject<Net.ChatLog>();
			Debug.Log(chatLog.name + "/" + chatLog.msg);
		}
	}

	public static UniTask<bool> UploadFile(string id, string password, string name, string title, string path, string idLang)
	{
		Net.<UploadFile>d__13 <UploadFile>d__;
		<UploadFile>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
		<UploadFile>d__.id = id;
		<UploadFile>d__.password = password;
		<UploadFile>d__.name = name;
		<UploadFile>d__.title = title;
		<UploadFile>d__.path = path;
		<UploadFile>d__.idLang = idLang;
		<UploadFile>d__.<>1__state = -1;
		<UploadFile>d__.<>t__builder.Start<Net.<UploadFile>d__13>(ref <UploadFile>d__);
		return <UploadFile>d__.<>t__builder.Task;
	}

	public static UniTask<FileInfo> DownloadFile(Net.DownloadMeta item, string path, string idLang)
	{
		Net.<DownloadFile>d__15 <DownloadFile>d__;
		<DownloadFile>d__.<>t__builder = AsyncUniTaskMethodBuilder<FileInfo>.Create();
		<DownloadFile>d__.item = item;
		<DownloadFile>d__.path = path;
		<DownloadFile>d__.idLang = idLang;
		<DownloadFile>d__.<>1__state = -1;
		<DownloadFile>d__.<>t__builder.Start<Net.<DownloadFile>d__15>(ref <DownloadFile>d__);
		return <DownloadFile>d__.<>t__builder.Task;
	}

	public static UniTask<List<Net.DownloadMeta>> GetFileList(string idLang)
	{
		Net.<GetFileList>d__16 <GetFileList>d__;
		<GetFileList>d__.<>t__builder = AsyncUniTaskMethodBuilder<List<Net.DownloadMeta>>.Create();
		<GetFileList>d__.idLang = idLang;
		<GetFileList>d__.<>1__state = -1;
		<GetFileList>d__.<>t__builder.Start<Net.<GetFileList>d__16>(ref <GetFileList>d__);
		return <GetFileList>d__.<>t__builder.Task;
	}

	public static UniTask<bool> SendVote(int id, string idLang)
	{
		Net.<SendVote>d__17 <SendVote>d__;
		<SendVote>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
		<SendVote>d__.id = id;
		<SendVote>d__.idLang = idLang;
		<SendVote>d__.<>1__state = -1;
		<SendVote>d__.<>t__builder.Start<Net.<SendVote>d__17>(ref <SendVote>d__);
		return <SendVote>d__.<>t__builder.Task;
	}

	public static UniTask<List<Net.VoteLog>> GetVote(string idLang)
	{
		Net.<GetVote>d__18 <GetVote>d__;
		<GetVote>d__.<>t__builder = AsyncUniTaskMethodBuilder<List<Net.VoteLog>>.Create();
		<GetVote>d__.idLang = idLang;
		<GetVote>d__.<>1__state = -1;
		<GetVote>d__.<>t__builder.Start<Net.<GetVote>d__18>(ref <GetVote>d__);
		return <GetVote>d__.<>t__builder.Task;
	}

	public static UniTask<bool> SendChat(string name, string msg, ChatCategory cat, string idLang)
	{
		Net.<SendChat>d__19 <SendChat>d__;
		<SendChat>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
		<SendChat>d__.name = name;
		<SendChat>d__.msg = msg;
		<SendChat>d__.cat = cat;
		<SendChat>d__.idLang = idLang;
		<SendChat>d__.<>1__state = -1;
		<SendChat>d__.<>t__builder.Start<Net.<SendChat>d__19>(ref <SendChat>d__);
		return <SendChat>d__.<>t__builder.Task;
	}

	public static UniTask<List<Net.ChatLog>> GetChat(ChatCategory cat, string idLang)
	{
		Net.<GetChat>d__20 <GetChat>d__;
		<GetChat>d__.<>t__builder = AsyncUniTaskMethodBuilder<List<Net.ChatLog>>.Create();
		<GetChat>d__.idLang = idLang;
		<GetChat>d__.<>1__state = -1;
		<GetChat>d__.<>t__builder.Start<Net.<GetChat>d__20>(ref <GetChat>d__);
		return <GetChat>d__.<>t__builder.Task;
	}

	public List<Net.ChatLog> chatList;

	private const string urlScript = "http://ylva.php.xdomain.jp/script/";

	private const string urlChat = "http://ylva.php.xdomain.jp/script/chat/";

	private const string urlVote = "http://ylva.php.xdomain.jp/script/vote/";

	private const string urlUpload = "http://ylva.php.xdomain.jp/script/uploader/";

	public static bool isUploading;

	public class ChatLog
	{
		public string name;

		public string msg;
	}

	public class VoteLog
	{
		public string name;

		public int count;

		public int index;

		public int time;
	}

	public class DownloadMeta
	{
		public bool IsValidVersion()
		{
			return !global::Version.Get(this.version).IsBelow(EClass.core.versionMoongate);
		}

		public string name;

		public string title;

		public string id;

		public string path;

		public string date;

		public int version;
	}

	public class DownloadCahce
	{
		public Dictionary<string, string> items = new Dictionary<string, string>();
	}
}
