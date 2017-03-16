using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public interface IStatisticsUpdater
    {
        void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext);
    }
}
