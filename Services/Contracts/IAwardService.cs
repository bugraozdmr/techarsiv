using Entities.Models;

namespace Services.Contracts;

public interface IAwardService
{
    // burda sadece çekme olacağı için uzatmadık -- ayrıca bu olmadan sorgu olmuyor
    IQueryable<Awards> Awards { get; }
}