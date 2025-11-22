using System;

namespace GameplayTimeTracker.Models;

public class BaseDataModel
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}