using System.Collections.Generic;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;

namespace GameplayTimeTracker.Validators;

public class PlaytimeValidator
{
    public IEnumerable<string> Validate(Playtime playtime)
    {
        if (playtime == null)
            yield return "Playtime is null";

        if (playtime?.GameId == 0)
            yield return "Playtime has no GameId";

        if (playtime != null && !GlobalServices.Repositories.GameRepository.ExistsById(playtime.GameId))
            yield return "Game does not exist";

        if (playtime != null && playtime.StartDate >= playtime.EndDate)
            yield return "Start date must be before end date";
    }
}