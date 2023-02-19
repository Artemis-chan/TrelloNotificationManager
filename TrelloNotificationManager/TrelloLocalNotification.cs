using System.Text;
using Manatee.Trello;
using Newtonsoft.Json;

namespace TrelloNotificationManager;

public class TrelloLocalNotification
{
    private readonly IMe _member;
    private readonly string _creatorNameEndpoint;
    private readonly List<int> _shownUnreadNotifications = new ();
    private readonly string processName;

    public TrelloLocalNotification(IMe member, MemberData data)
    {
        _member = member;
        var auth = data.Auth;
        processName = data.Executable;
        _creatorNameEndpoint = $"https://api.trello.com/1/notifications/{{0}}/memberCreator/fullName?key={auth.AppKey}&token={auth.UserToken}";
    }

    public async Task Update()
    {
        await _member.Notifications.Refresh();
        foreach (var notif in _member.Notifications)
        {
            var idHash = notif.Id.GetHashCode();
            if(_shownUnreadNotifications.Contains(idHash))
                continue;
            await Print(notif);
            _shownUnreadNotifications.Add(idHash);
        }
        CleanShownList();
    }

    private void CleanShownList()
    {
        if(!_shownUnreadNotifications.Any()) return;
        var _toRemove = _shownUnreadNotifications.Where(n => _member.Notifications.All(_ => _.Id.GetHashCode() != n)).ToArray();
        foreach (var n in _toRemove)
        {
            _shownUnreadNotifications.Remove(n);
        }
    }

    private async Task Print(INotification notification)
    {
        // return;
        var data = notification.Data;
        var title = $"{_member.FullName} : {data?.Board?.Name}";
        Console.WriteLine(title);
        
        var sb = new StringBuilder();
        if(notification.Creator is null)
        {
            sb.Append(await GetCreatorName(notification));
        }
        sb.Append(notification);
        var body = sb.ToString();
        Console.WriteLine(body);
        
        //show notification
        var link = data?.GetLink();
        Program.NotificationList.ShowNotification(title, body, LinkData.New(link, processName));
        if(link is null) return;
        Console.WriteLine(link);
    }
    
    private async Task<string> GetCreatorName(INotification notification)
    {
        var url = string.Format(_creatorNameEndpoint, notification.Id);
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<UserName>(content);
    }

    private struct UserName
    {
        [JsonProperty]
        public string _value;

        public UserName() => _value = string.Empty;

        public static implicit operator string(UserName name) => name._value;
    }
}