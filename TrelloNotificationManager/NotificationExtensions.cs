﻿using System.Reflection;
using Manatee.Trello;

public static class NotificationExtensions
{
    public enum UneadFilter
    {
        all,
        unread,
        read
    }
    public static void ReadFilter(this IReadOnlyNotificationCollection col, UneadFilter filter)
    {
        if(col is not ReadOnlyNotificationCollection) return;
        var prop = typeof(ReadOnlyNotificationCollection).GetProperty("AdditionalParameters", BindingFlags.NonPublic | BindingFlags.Instance);
        if(prop?.GetValue(col) is not Dictionary<string, object> additionalParams) return;
        additionalParams["read_filter"] = filter.ToString();
    }
    
    public static string? GetLink(this INotificationData notification) => notification.Card?.GetLink() ?? notification.Board?.GetLink();
    public static string? GetLink(this IBoard board) => board.ShortLink is null ? null : $"https://trello.com/b/{board.ShortLink}";
    public static string? GetLink(this ICard card) => card.ShortUrl;
    
    // public static void ReadAdditonalParams(this IReadOnlyNotificationCollection col)
    // {
    //     if(col is not ReadOnlyNotificationCollection) return;
    //     var prop = typeof(ReadOnlyNotificationCollection).GetProperty("AdditionalParameters", BindingFlags.NonPublic | BindingFlags.Instance);
    //     if(prop is null) return;
    //     // var additionalParams = prop.GetGetMethod(true)?.MakeGenericMethod(typeof(string), typeof(object)).Invoke(col, Array.Empty<object?>()) as Dictionary<string, object>;
    //     var additionalParams = prop.GetValue(col) as Dictionary<string, object>;
    //     Console.WriteLine("value: " + additionalParams?.Count);
    // }
}