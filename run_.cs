using System;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;

namespace c_selfbot
{
    class run_
    {
        public static void run(settings args) {
            try {
                context.settings = args;

                context.client.OnLoggedIn += on_logged_in_event;

                context.client.OnMessageReceived += on_message_received_event;

                context.client.CreateCommandHandler(context.settings.prefix, true);

                context.client.Login(context.settings.discord_token);

                other.watermark();

                System.Threading.Thread.Sleep(-1);
            }
            catch (InvalidTokenException) {
                Console.WriteLine("invalid discord token in the config file!!");
                Console.ReadLine();
            }
        }

        private static void on_logged_in_event(DiscordSocketClient client, LoginEventArgs args) =>
            Console.WriteLine($"logged in successfully as {args.User.Username} ");

        private static void on_message_received_event(DiscordSocketClient client, MessageEventArgs args) {
            var message = args.Message;
            var is_owner = message.Author.User.Id == client.User.Id;
            var is_command = message.Content.StartsWith(context.settings.prefix);
                
            if (is_command && is_owner)
                message.Delete();

            if (message.Mentions.Count > 0) 
                foreach (var user in message.Mentions) 
                    if (!is_owner && user.Id == client.User.Id)
                        Console.WriteLine(
                            $"{message.Author.User.Username} => {other.replace_mentions_with_nicks(client, message.Content)} (#{client.GetChannel(message.Channel.Id).Name} | {client.GetGuild(message.Guild.Id).Name}) ");

            var channel_id = message.Channel.Id;

            if (!is_owner && client.GetChannel(channel_id).Type == ChannelType.DM && context.is_afk) {
                Thread.Sleep(2000);
                client.SendMessage(channel_id, context.settings.afk_message);
            }

            if (is_owner && !is_command && context.uwu_mode && (message.Content.Contains("u") || message.Content.Contains("o")))
                message.Edit(message.Content.Replace('u', 'w').Replace("o", "owo"));
            
            if (context.settings.nitro_sniper) {
                var rgx_match = new Regex("(discord.gift/|discord.com/gifts/|discordapp.com/gifts/)([\\w]+)").Match(message.Content);

                if (rgx_match.Success) {
                    var gift = rgx_match.Groups[2].Value;

                    if (gift.Length >= 16) {
                        try {
                            client.RedeemNitroGift(gift, channel_id);
                            
                            Console.WriteLine($"nitro gift redeemed successfully! : {gift} ");
                        }
                        catch (DiscordHttpException ex) {
                            switch (ex.Code) { //originally by ilinked
                                case DiscordError.NitroGiftRedeemed:
                                    Console.WriteLine($"nitro gift was already redeemed : {gift} ");
                                    break;
                                case DiscordError.UnknownGiftCode:
                                    Console.WriteLine($"invalid nitro gift : {gift} ");
                                    break;
                                default:
                                    Console.WriteLine($"unknown error: {ex.Code} | {ex.ErrorMessage}");
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
