using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public interface IStatisticsUpdater
    {
        void Update(MatchInfo info, DatabaseContext databaseContext);
    }
}
