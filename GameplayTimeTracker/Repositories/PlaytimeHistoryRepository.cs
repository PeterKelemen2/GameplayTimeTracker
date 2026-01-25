using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeHistoryRepository(AppDbContext db) : Repository<PlaytimeHistory>(db)
{
    public PlaytimeHistory GetAppliedHistoryItem(int gameId, DateTime date, TimeSpan duration)
    {
        var historyItem =
            Db.PlaytimeHistories.FirstOrDefault(x => x.GameId == gameId && x.Date.Date == date.Date);

        if (historyItem == null)
        {
            historyItem = new PlaytimeHistory
            {
                GameId = gameId,
                Date = date,
                TotalSeconds = (int)duration.TotalSeconds,
                CreatedOn = DateTime.Now
            };
        }
        else
        {
            historyItem.TotalSeconds += (int)duration.TotalSeconds;
        }

        Console.WriteLine(historyItem);

        return historyItem;
    }

    public void AddToPlaytimeHistoryByPlaytime(Playtime playtime)
    {
        var dayCursor = playtime.StartDate;

        List<PlaytimeHistory> playtimeHistories = new List<PlaytimeHistory>();

        while (dayCursor.Date <= playtime.EndDate)
        {
            bool isFirstDay = dayCursor.Date == playtime.StartDate.Date;
            bool isLastDay = dayCursor.Date == playtime.EndDate.Date;

            DateTime chunkStart = isFirstDay ? playtime.StartDate : dayCursor.Date;
            DateTime chunkEnd = isLastDay ? playtime.EndDate : dayCursor.Date.AddDays(1);

            var duration = chunkEnd - chunkStart;

            if (duration.TotalSeconds > 0)
            {
                var historyItem = GetAppliedHistoryItem(playtime.GameId, dayCursor.Date, duration);
                playtimeHistories.Add(historyItem);
            }

            dayCursor = dayCursor.Date.AddDays(1);
        }

        foreach (var historyItem in playtimeHistories)
        {
            if (historyItem.Id == 0)
            {
                Db.PlaytimeHistories.Add(historyItem);
            }
            else
            {
                Db.PlaytimeHistories.Update(historyItem);
            }
        }

        Db.SaveChanges();
    }
}