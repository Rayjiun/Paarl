using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paarl.Utility
{
    internal class Location
    {
        /// <summary>
        /// Gets the Black Ops Root Path
        /// </summary>
        /// <returns></returns>
        public static string? GetBlackOpsRoot()
        {
            return Environment.GetEnvironmentVariable("TA_GAME_PATH");
        }
    }
}
