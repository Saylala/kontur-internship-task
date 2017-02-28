using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public interface IStatisticsUpdater
    {
        void Update(MatchInfo info);
    }
}
