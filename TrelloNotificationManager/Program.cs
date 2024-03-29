﻿using System.Diagnostics;
using ManagedBass;
using Manatee.Trello;
using Modern.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrelloNotificationManager;

internal sealed class Program
{
	const string AuthFile = @"members.json";
	const string NotificationFile = @"Resources\Pickup_coin_27.mp3";
	static readonly TrelloFactory Trello = new ();

	public static NotificationListForm NotificationList = null!;
	
	private static int _stream;
	private static bool _running = true;

	[STAThread]
	private static void Main(string[] args)
	{
		if(!File.Exists(AuthFile))
		{
			CreateAuthFile();
			return;
		}
		

		//Init Bass
		if (!Bass.Init())
		{
			Console.WriteLine("Could not init audio");
			return;
		}
		_stream = Bass.CreateStream(NotificationFile);
		if (_stream == 0)
		{
			Console.WriteLine("Could not sound");
			return;
		}

		//Handle exit
		Console.CancelKeyPress += (s, e) =>
		{
			Console.WriteLine("Closing..");
			_running = false;
			Bass.StreamFree(_stream);
			Bass.Free();
		};
		
		var auths = JsonConvert.DeserializeObject<MemberData[]>(File.ReadAllText(AuthFile));
		if(auths is null) return;
		NotificationList = new NotificationListForm();
		Task.Run(() => PrintNotifications(auths));
		Application.Run(NotificationList);
	}

	public static void PlayNotificationSound()
	{
		Bass.ChannelPlay(_stream);
	}

	private static void CreateAuthFile()
	{
		Console.WriteLine("No auth file found");
		File.AppendAllText(AuthFile, JsonConvert.SerializeObject(new[] { new MemberData
		{
			Auth = new TrelloAuthorization()
		} }, Formatting.Indented));
		new Process
		{
			StartInfo = new ProcessStartInfo(AuthFile)
			{
				UseShellExecute = true
			}
		}.Start();
	}

	private static async Task PrintNotifications(MemberData[] datas)
	{
		// TrelloAuthorization.Default.AppKey = auths[0].AppKey;
		// TrelloAuthorization.Default.UserToken = auths[0].UserToken;
		List<TrelloLocalNotification> members = new ();
		foreach (var data in datas)
		{
			var m = await Trello.Me(data.Auth);
			if(m is null) continue;
			members.Add(new TrelloLocalNotification(m, data));
			m.Notifications.ReadFilter(NotificationExtensions.UneadFilter.unread);
			// var n = Trello.Notification(m.Notifications[0].Id, auth);
		}
		// NotificationList.Hide();
		while (_running)
		{
			foreach (var m in members)
			{
				await m.Update();
			}
			Thread.Sleep(2000);
		}
	}
}

[Serializable]
public struct MemberData
{
	public TrelloAuthorization Auth;
	public string Executable;
}