using System.Diagnostics;
using Manatee.Trello;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class Program
{
	const string AuthFile = @"members.json";
	static readonly TrelloFactory Trello = new ();
	private static void Main(string[] args)
	{
		if(!File.Exists(AuthFile))
		{
			Console.WriteLine("No auth file found");
			File.AppendAllText(AuthFile, JsonConvert.SerializeObject(new []{ new TrelloAuthorization() }, Formatting.Indented));
			new Process
			{
				StartInfo = new ProcessStartInfo(AuthFile)
				{
					UseShellExecute = true
				}
			}.Start();
			
			return;
		}
		
		var auths = JsonConvert.DeserializeObject<TrelloAuthorization[]>(File.ReadAllText(AuthFile));
		if(auths is null) return;
		PrintNotifications(auths).Wait();
	}

	private static async Task PrintNotifications(TrelloAuthorization[] auths)
	{
		// TrelloAuthorization.Default.AppKey = auths[0].AppKey;
		// TrelloAuthorization.Default.UserToken = auths[0].UserToken;
		List<TrelloLocalNotification> members = new ();
		foreach (var auth in auths)
		{
			var m = await Trello.Me(auth);
			if(m is null) continue;
			members.Add(new TrelloLocalNotification(m, auth));
			m.Notifications.ReadFilter(NotificationFilter.UneadFilter.unread);
			// var n = Trello.Notification(m.Notifications[0].Id, auth);
		}
		while (true)
		{
			foreach (var m in members)
			{
				await m.Update();
			}
			Thread.Sleep(2000);
		}
	}
}