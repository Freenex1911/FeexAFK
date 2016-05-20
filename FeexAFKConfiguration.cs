using Rocket.API;

namespace Freenex.FeexAFK
{
    public class FeexAFKConfiguration : IRocketPluginConfiguration
    {
        public int Seconds;
        public int CheckInterval;
        public bool MessageEnabled;
        public bool KickEnabled;
        public int KickMinPlayers;
        public bool IgnoreAdmins;

        public void LoadDefaults()
        {
            Seconds = 300;
            CheckInterval = 1000;
            MessageEnabled = true;
            KickEnabled = false;
            KickMinPlayers = 0;
            IgnoreAdmins = true;
        }
    }
}
