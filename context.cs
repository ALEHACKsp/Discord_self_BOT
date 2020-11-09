using Discord.Gateway;

namespace c_selfbot
{
    public static class context
    {
        public static DiscordSocketClient client = new DiscordSocketClient();

        public static settings settings = new settings();
        
        public static bool is_afk { get; set; }
        
        public static bool uwu_mode { get; set; }
    }
}