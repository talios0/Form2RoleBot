﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class SheetsFunctionality
    {
        public static bool FindUsername(SocketGuildUser user, IList<object> userCell) // Checks if the username from the Google Sheets matches a discord user
        {
            string username = "NaN";
            if (Config.GoogleData.DiscordIDField != -1)
            {
                username = userCell[Config.GoogleData.DiscordIDField].ToString();
                username = username.Trim(); // trims excess characters
                username = username.Replace(" ", "");
                if (username != user.Username + "#" + user.Discriminator &&
                    username != user.Discriminator &&
                    username != user.Nickname + "#" +
                    user.Discriminator // Nickname is here juserst in case, busert it is probably one of the worst ways of doing this since it'll change once the nickname userpdates
                ) return false;
            }
            else
            {
                username = userCell[0].ToString();
                username = username.Trim();
                username = username.Replace(" ", "");
                if (username != user.Username + "#" + user.Discriminator &&
                    username != user.Discriminator &&
                    username != user.Nickname + "#" + user.Discriminator
                ) return false;
            }

            return true;
        }


        public static string[] SeperateRoles(string role)
        {
            if (role.Contains(","))
            {
                role = role.Replace(" ", "");
                return role.Split(',');
            }
            else if (role.Contains("+"))
            {
                role = role.Replace(" ", "");
                return role.Split('+');
            }

            string[] seperatedRole = new string[1];
            seperatedRole[0] = role;
            return seperatedRole;
        }

        public static async Task CheckAndCreateRole(SocketGuild guild, string role)
        {
            bool roleFound = false;
            foreach (SocketRole dRole in guild.Roles)
            {
                if (dRole.Name.Equals(role))
                {
                    roleFound = true;
                    continue;
                }
            }
            if (!roleFound)
            {
                await guild.CreateRoleAsync(role);
            }
        }

        public static async Task<SocketRole> CreateRole(SocketGuild guild, string role)
        {
            await guild.CreateRoleAsync(role);
            return guild.Roles.First(x => x.Name == role);
        }

        public static async Task RemoveRole(SocketGuildUser user, string role)
        {
            foreach (string roleGroup in Config.RoleGroup.Groups)
            {
                foreach (SocketRole userRole in user.Roles) // Checks all roles assigned to the user
                {
                    if (roleGroup.Contains(userRole.Name) && roleGroup.Contains(role)) // Checks for overlapping roles (from roleGroups.json)
                    {
                        // Removes the user's current role
                        await user.RemoveRoleAsync(userRole);
                    }
                }
            }
        }

        public static async Task FindAndSetNickname(SocketGuildUser user, IList<object> userCell)
        {
            string nickname;

            if (Config.GoogleData.NicknameField == -2) // finds the nickname in the Google Sheets data
                return;

            if (Config.GoogleData.NicknameField != -1)
                nickname = userCell[Config.GoogleData.NicknameField].ToString();
            else
                nickname = userCell[userCell.Count - 1].ToString();

            await SetNickname(user, nickname);
        }

        public static async Task SetNickname(SocketGuildUser user, string nickname) // sets the user's nickname
        {
            try
            {
                // sets nickname
                await user.ModifyAsync(x =>
                {
                    x.Nickname = nickname;
                });
            }
            catch
            {
                // occurs when the user is ranked above the bot
                Console.WriteLine("No nickname was specified or their rank is too high.");
            }

        }
    }
}
