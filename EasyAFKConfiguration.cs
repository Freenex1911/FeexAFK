using Rocket.API;

namespace Freenex.EasyAFK
{
    public class EasyAFKConfiguration : IRocketPluginConfiguration
    {
        public int afkSeconds;
        public bool afkKick;
        public int afkCheckInterval;

        public void LoadDefaults()
        {
            afkSeconds = 300;
            afkKick = false;
            afkCheckInterval = 1000;
        }
    }
}
