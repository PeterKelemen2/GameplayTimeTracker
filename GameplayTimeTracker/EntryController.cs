namespace GameplayTimeTracker;

public class EntryController
{
    EntryRepository repository;

    public EntryController()
    {
        repository = new EntryRepository();
    }
}