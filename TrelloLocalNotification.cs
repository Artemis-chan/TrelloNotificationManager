using System.Text;
using Manatee.Trello;
using Newtonsoft.Json;

public class TrelloLocalNotification
{
    private readonly IMe _member;
    private readonly string _creatorNameEndpoint;
    private readonly List<int> _shownUnreadNotifications = new ();

    public TrelloLocalNotification(IMe member, TrelloAuthorization auth)
    {
        _member = member;
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
        var sb = new StringBuilder();
        var data = notification.Data;
        sb.Append($"{_member.FullName} : {data?.Board?.Name}\n");
        if(notification.Creator is null)
        {
            sb.Append(await GetCreatorName(notification));
        }
        sb.Append(notification);
        var m = sb.ToString();
        Console.WriteLine(m);
        
        //show notification
        var link = data?.GetLink();
        if(link is null) return;
        Console.WriteLine(link);
        Program.NotificationList.ShowNotification(m, link);
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
        public static implicit operator string(UserName name) => name._value;
    }
}