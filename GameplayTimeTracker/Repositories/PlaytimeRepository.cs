using System;
using System.Collections.Generic;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Helpers;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeRepository : Repository<Playtime>
{
    public PlaytimeRepository(AppDbContext db) : base(db)
    {
    }

    public void AddPlaytime(Playtime playtime)
    {
        if (playtime == null)
        {
            Console.WriteLine("Playtime is null");
            return;
        }

        if (playtime.GameId == 0)
        {
            Console.WriteLine("Playtime has no GameId!");
            return;
        }

        if (playtime.Id == 0)
        {
            playtime.CreatedOn = DateTime.Now;
        }

        if (playtime.StartDate >= playtime.EndDate)
        {
            Console.WriteLine("Playtime dates incorrect!");
            return;
        }

        var today = DateTime.Now;
        int daysBetween = (playtime.StartDate - playtime.EndDate).Days;

        var startDate = playtime.StartDate;

        if (daysBetween >= 1)
        {
            DateTime currentDay = startDate.Date;
            DateTime midnight = currentDay.AddDays(1);

            TimeSpan toMidnight = midnight - currentDay;
            var dur = PlaytimeHelper.NormalizeTime(toMidnight);
            var existingHistoryItem =
                GlobalServices.Repositories.PlaytimeHistoryRepository.GetHistoryItemWithDate(currentDay);

            if (existingHistoryItem != null)
            {
                // Update item with added playtime
            }
            else
            {
                // Create new with playtime
            }
            // https://github.com/PeterKelemen2/GameplayTimeTracker/blob/rework/GameplayTimeTracker/Model/Entry.cs ~350
        }

        // _db.Playtimes.Add(playtime);
        // _db.SaveChanges();
    }
}