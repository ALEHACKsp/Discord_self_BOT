using System;

namespace c_selfbot
{
    class main
    {
        static void Main(string[] args) {
            Console.WriteLine("--> Discord Self BOT <--");
            Console.WriteLine("choose a option : ");
            Console.WriteLine("1 - run selfbot");
            Console.WriteLine("2 - load custom cfg & run");
            Console.WriteLine("3 - first setup");

            int option = Convert.ToInt32(Console.ReadLine());
            switch (option) {
                case 1:
                    Console.Clear();
                    run_.run(config.load_config(config.cfg));

                    break;

                case 2:
                    other.watermark();
                    Console.WriteLine("write the config file path : ");

                    run_.run(config.load_config(Console.ReadLine()));
                    break;

                case 3:
                    other.watermark();

                    settings _default = new settings();

                    Console.WriteLine("token : ");
                    _default.discord_token = Console.ReadLine();

                    other.watermark();

                    Console.WriteLine("prefix : ");
                    _default.prefix = Console.ReadLine();

                    other.watermark();

                    Console.WriteLine("nitro sniper enabled : (y, n)");
                    _default.nitro_sniper = (Console.ReadLine() == "y") ? true : false;

                    other.watermark();
                    
                    Console.WriteLine("afk message : ");
                    _default.afk_message = Console.ReadLine();

                    other.watermark();

                    Console.WriteLine("stream link ( activity ) : ");
                    _default.stream_link = Console.ReadLine();

                    config.save_config(config.cfg, _default);

                    Console.Clear();

                    Main(null);
                    break;
            }
        }
    }
}
