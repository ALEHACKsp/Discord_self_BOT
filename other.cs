using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Gateway;

namespace c_selfbot
{
    class other
    {
        public static void watermark() {
            Console.Clear();
            Console.WriteLine("--> Discord self BOT <--");
        }

        public static string md5(string input) {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder str_b = new StringBuilder();

            for (int i = 0;i < data.Length;i++) {
                str_b.Append(data[i].ToString("x2"));
            }

            return str_b.ToString();
        }

        public static string sha256(string input) {
            byte[] data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder str_b = new StringBuilder();

            for (int i = 0;i < data.Length;i++) {
                str_b.Append(data[i].ToString("x2"));
            }

            return str_b.ToString();
        }

        public static string xor_str(string input, string key) {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
                sb.Append((char)(input[i] ^ key[(i % key.Length)]));

            return sb.ToString();
        }
        public static string byte_arr_to_str(byte[] ba) {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] str_to_byte_arr(String hex) {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0;i < NumberChars;i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        //returns the encrypted string encoded using hex
        public static string aes256(string input, string key) {
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = Encoding.UTF8.GetBytes(key);

            using (MemoryStream memoryStream = new MemoryStream()) {
                using (ICryptoTransform aesEncryptor = encryptor.CreateEncryptor()) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write)) {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(input);

                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);

                        cryptoStream.FlushFinalBlock();

                        byte[] cipherBytes = memoryStream.ToArray();

                        return byte_arr_to_str(cipherBytes);
                    }
                }
            }
        }

        public static string deaes256(string input, string key) {
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;
            encryptor.Key = Encoding.UTF8.GetBytes(key);

            using (MemoryStream memoryStream = new MemoryStream()) {
                using (ICryptoTransform aesDecryptor = encryptor.CreateDecryptor()) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write)) {
                        byte[] cipherBytes = str_to_byte_arr(input);

                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                        cryptoStream.FlushFinalBlock();

                        byte[] plainBytes = memoryStream.ToArray();

                        return Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
                    }
                }
            }
        }

        public static string replace_mentions_with_nicks(DiscordSocketClient client, string msg) {
            var msg_matches = new Regex("(<@|<@!)(\\d+)(>)").Matches(msg);
            //ghetto?!?!

            foreach (Match match in msg_matches)
                msg = msg.Replace(match.Value, "@" + client.GetUser(Convert.ToUInt64(match.Groups[2].Value)).Username);

            return msg;
        }
        
    }
}
