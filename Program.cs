using Manatee.Trello;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class Program
{
	const string AuthFile = @"Z:\Documents\programmin\cs\TrelloNotificationManager\members.json";
	static readonly TrelloFactory Trello = new ();
	private static void Main(string[] args)
	{
		var auths = JsonConvert.DeserializeObject<TrelloAuthorization[]>(File.ReadAllText(AuthFile));
		// var notifs = new SearchQuery().Member()
		// Console.WriteLine("Hello, World!");
		if(auths is null) return;
		PrintNotifications(auths).Wait();
	}

	private static async Task PrintNotifications(TrelloAuthorization[] auths)
	{
		TrelloAuthorization.Default.AppKey = auths[0].AppKey;
		TrelloAuthorization.Default.UserToken = auths[0].UserToken;
		List<TrelloLocalNotification> members = new ();
		foreach (var auth in auths)
		{
			var m = await Trello.Me();
			if(m is null) continue;
			members.Add(new TrelloLocalNotification(m, auth));
			m.Notifications.ReadFilter(NotificationFilter.UneadFilter.unread);
			// var n = Trello.Notification(m.Notifications[0].Id, auth);
		}
		while (true)
		{
			Thread.Sleep(2000);
			foreach (var m in members)
			{
				await m.Update();
			}
		}
	}
}