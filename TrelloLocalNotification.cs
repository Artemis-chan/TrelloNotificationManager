using Manatee.Trello;
using Newtonsoft.Json;

public class TrelloLocalNotification
{
    private IMe _member;
    private string _creatorNameEndpoint;
    private List<int> _shownUnreadNotifications = new ();

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
        
        
        // foreach (var n in _shownUnreadNotifications)
        // {
        //     if (_member.Notifications.Any(_.Id.GetHashCode() == n))
        //     {
        //         _shownUnreadNotifications.Remove(n);
        //     }
        // }
    }

    private async Task Print(INotification notification)
    {
        var s = notification.ToString();
        if(notification.Creator is null)
        {
            s = await GetCreatorName(notification) + s;
        }
        Console.WriteLine(s);
    }
    
    private async Task<string> GetCreatorName(INotification notification)
    {
        var url = string.Format(_creatorNameEndpoint, notification.Id);
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<UserName>(content);
    }

    [Serializable]
    private struct UserName
    {
        public string _value;
        
        public static implicit operator string(UserName name) => name._value;
    }
}