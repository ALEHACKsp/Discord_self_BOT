using Discord;
using Discord.Gateway;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Leaf.xNet;
using Newtonsoft.Json;

namespace c_selfbot {
    [Command("help", "the help command")]
    public class c_help : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            Console.WriteLine("im too lazy to do this command, please do it for me ;D");
        }
    }

    [Command("save_config", "updates and save the current config")]
    public class c_save_config : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            config.save_config(args[0], context.settings);
            Console.WriteLine("saved the config successfully");
        }
    }

    [Command("load_config", "load a custom config without re-opening the selfbot")]
    public class c_load_config : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            context.settings = config.load_config(args[0]);
            Console.WriteLine("loaded the config successfully");
        }
    }

    [Command("clear_console", "clear the console")]
    public class c_clear_console : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            other.watermark();
            Console.WriteLine("cleared the console!");
        }
    }

    [Command("purge", "purges text youve written in the channel you wrote the command")]
    public class c_purge : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            Console.WriteLine("purging messages...");
            
            var msgs = (args[0] == "all")
                ? client.GetChannelMessages(message.Channel.Id, new MessageFilters {
                    UserId = client.User.Id
                })
                : client.GetChannelMessages(message.Channel.Id,
                    new MessageFilters {
                        Limit = Convert.ToUInt32(args[0]),
                        UserId = client.User.Id
                    });

            foreach (var mess in msgs) {
                Console.WriteLine("message deleted : " + mess.Content);

                mess.Delete();
            }
        }
    }

    [Command("set_activity", "set the activity with a custom one")]
    public class c_set_activity : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            int act_type = Convert.ToInt32(args[0]);

            //the args array is a array split by a space, in the thing below, i exclude the first argument and grab the others to 'enable' spaces
            var act_name = String.Join(" ", args.Where(val => val != args[0]).ToArray());

            if (act_type == 1)
                client.SetActivity(new StreamActivity() {Url = context.settings.stream_link, Name = act_name});
            else
                client.SetActivity(new Activity() {Type = (ActivityType) act_type, Name = act_name});

            Console.WriteLine("your activity now is : " + act_name);
        }
    }

    [Command("reset_activity", "reset the current activity")]
    public class c_reset_activity : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            client.SetActivity(new Activity() {Type = 0, Name = null});
            Console.WriteLine("your activity now has been reseted");
        }
    }

    [Command("userinfo", "get the info of a user")]
    public class c_userinfo : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            if (message.Mentions.Count > 0) {
                //foreach user in the message mentions, send a embed with the data, if there is a better way of doing that, please make a issue to inform me :(
                //ik i could do 'client.GetUser(id)' but when i mention someone i get smth like @<!id> and i dont wanna parse this mention to an id
                foreach (var user in message.Mentions) {

                    EmbedMaker embed = new EmbedMaker() {
                        Title = $"{user.Username}#{user.Discriminator}",
                        ImageUrl = user.Avatar.Url,
                        Description = $"Type : {user.Type.ToString()}\n User ID : {user.Id}",
                    };

                    embed.Author.Name = "User Data";
                    embed.Footer.Text = $"Created At : {user.CreatedAt.DateTime.ToShortDateString()}";
                    embed.Author.Url = "https://github.com/k0ez13/Discord_self_BOT";

                    Console.WriteLine("sent the userinfo!!");

                    client.SendMessage(message.Channel.Id, "", false, embed);
                }
            }
            else {
                Console.WriteLine("you have to mention someone in the command");
            }
        }
    }

    [Command("serverinfo", "get the info of the server")]
    public class c_serverinfo : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            Guild guild = client.GetGuild(message.Guild.Id);

            EmbedMaker embed = new EmbedMaker() {
                Title = guild.Name,
                ImageUrl = guild.Icon.Url,
                Description =
                    $"Server ID : {guild.Id.ToString()}\n Owner Name : {client.GetUser(guild.OwnerId).Username}\n Region : {guild.Region.ToUpper()}"
            }; //fields looks bad

            embed.Author.Name = "Server Info";

            embed.Author.Url = "https://github.com/k0ez13/Discord_self_BOT";

            Console.WriteLine("sent the serverinfo!");

            client.SendMessage(message.Channel.Id, "", false, embed);
        }
    }
    
    //there are string encryptors too here, i just like the name 'hash' for the command
    [Command("hash", "hash a certain string with a certain argument")]
    public class c_hash : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            ulong id = message.Channel.Id;
            switch (args[0]) {
                case "md5":
                    client.SendMessage(id, other.md5(args[1]));
                    Console.WriteLine("Sent md5 hash!");
                    break;

                case "sha256":
                    client.SendMessage(id, other.sha256(args[1]));
                    Console.WriteLine("Sent sha256 hash!");
                    break;

                case "aes256":
                    try {
                        client.SendMessage(id, other.aes256(args[1], args[2]));
                        Console.WriteLine($"Sent aes256 encrypted string, key : {args[2]} !");
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"wrong key length / problem at encryption : {ex.Message}");
                    }

                    break;

                case "deaes256":
                    try {
                        client.SendMessage(id, other.deaes256(args[1], args[2]));
                        Console.WriteLine($"Sent aes256 decrypted string, key : {args[2]} !");
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"wrong key length / problem at encryption : {ex.Message}");
                    }

                    break;

                case "xor":
                    client.SendMessage(id, other.xor_str(args[1], args[2]));
                    Console.WriteLine($"Sent xor'd string!, key : {args[2]} !");
                    break;

                case "hex":
                    client.SendMessage(id, other.byte_arr_to_str(Encoding.UTF8.GetBytes(args[1])));
                    Console.WriteLine("Sent hex encoded string!");
                    break;

                case "dehex":
                    client.SendMessage(id, Encoding.UTF8.GetString(other.str_to_byte_arr(args[1])));
                    Console.WriteLine("Sent normal string!");
                    break;

                default:
                    Console.WriteLine("unknown type");
                    break;
            }
        }
    }

    [Command("embed", "embed a message with a certain text")]
    public class c_embed : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            var to_embed = String.Join(" ", args);

            EmbedMaker embed = new EmbedMaker() {Description = to_embed};

            Console.WriteLine("sent the embed!");

            client.SendMessage(message.Channel.Id, "", false, embed);
        }
    }

    [Command("ipinfo", "this command grabs the info of a ip using an api")]
    public class c_ipinfo : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            dynamic info =
                JsonConvert.DeserializeObject(new HttpRequest().Get($"http://ip-api.com/json/{args[0]}").ToString());
            if (info.status != "fail") {
                EmbedMaker embed = new EmbedMaker() {
                    Title = info.query,
                    Description =
                        $"Country : {info.country}\n Region : {info.regionName}\n City : {info.city}\n ISP : {info.isp}\n ORG : {info.org}",
                };

                embed.Author.Name = "IP Info";
                embed.Footer.Text = $"Status : {info.status}"; //useless, but looks better
                embed.Author.Url = "https://github.com/k0ez13/Discord_self_BOT";

                Console.WriteLine("sent the ipinfo!");

                client.SendMessage(message.Channel.Id, "", false, embed);
            }
            else {
                Console.WriteLine("the IP is not valid!");
            }
        }
    }

    [Command("uwu", "define the uwu mode true or not")]
    public class c_uwu : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            context.uwu_mode = !context.uwu_mode;
            
            Console.WriteLine($"uwu mode is {((context.uwu_mode) ? "enabled" : "disabled")}!");
        }
    }

    [Command("afk", "define the afk mode true or not")]
    public class c_afk : Command {
        public override void Execute(DiscordSocketClient client, string[] args, Message message) {
            context.is_afk = !context.is_afk;
            
            Console.WriteLine($"afk mode is {(context.is_afk ? "enabled" : "disabled")}!");
        }
    }
}
