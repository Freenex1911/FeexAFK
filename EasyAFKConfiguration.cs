using Rocket.API;

namespace Freenex.EasyAFK
{
    public class EasyAFKConfiguration : IRocketPluginConfiguration
    {
        public int afkSeconds;
        public bool afkCheckAdmins;
        public bool afkKick;
        public bool afkKickAdmins;
        public int afkKickMinPlayers;
        public int afkCheckInterval;
        

        public void LoadDefaults()
        {
            afkSeconds = 300;
            afkCheckAdmins = false;
            afkKick = false;
            afkKickAdmins = false;
            afkKickMinPlayers = 0;
            afkCheckInterval = 1000;
        }
    }
}
