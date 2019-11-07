using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WhalesFargo.Services;
using HtmlAgilityPack;
using System.Linq;
using System.Net.Http;

namespace WhalesFargo.Modules
{
    /**
     * AdminModule
     * Perform administrative level commands.
     */
    [Name("Admin")]
    [Summary("Admin module to manage this discord server.")]
    public class AdminModule : CustomModule
    {
        // Private variables
        private readonly AdminService m_Service;
        static readonly HttpClient client = new HttpClient();
        // Dependencies are automatically injected via this constructor.
        // Remember to add an instance of the service.
        // to your IServiceCollection when you initialize your bot!
        public AdminModule(AdminService service)
        {
            m_Service = service;
            m_Service.SetParentModule(this); // Reference to this from the service.
        }

        [Command("mute")]
        [Remarks("!mute [user]")]
        [Summary("This allows admins to mute users.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task MuteUser([Remainder] IGuildUser user)
        {
            await m_Service.MuteUser(Context.Guild, user);
        }

        [Command("unmute")]
        [Remarks("!unmute [user]")]
        [Summary("This allows admins to unmute users.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task UnmuteUser([Remainder] IGuildUser user)
        {
            await m_Service.UnmuteUser(Context.Guild, user);
        }

        [Command("kick")]
        [Remarks("!kick [user] [reason]")]
        [Summary("This allows admins to kick users.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, [Remainder] string reason = null)
        {
            await m_Service.KickUser(Context.Guild, user, reason);
        }

        [Command("ban")]
        [Remarks("!ban [user] [reason]")]
        [Summary("This allows admins to ban users.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, [Remainder] string reason = null)
        {
            await m_Service.BanUser(Context.Guild, user, reason);
        }

        [Command("addrole")]
        [Remarks("!addrole [user]")]
        [Summary("This allows admins to add specific roles to a user.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddRoleUser(IGuildUser user, [Remainder]string role)
        {
            await m_Service.AddRoleUser(Context.Guild, user, role);
        }

        [Command("removerole")]
        [Alias("removerole", "delrole")]
        [Remarks("!delrole [user]")]
        [Summary("This allows admins to remove specific roles to a user.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RemoveRoleUser(IGuildUser user, [Remainder]string role)
        {
            await m_Service.RemoveRoleUser(Context.Guild, user, role);
        }

        [Command("setprogress")]
        [Remarks("!setprogress [nick] [server] [region]")]
        [Summary("This command help to set roles")]
        [RequireUserPermission(GuildPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task SetScore([Remainder] string role = "")
        {
            role = role.ToLower();
            string[] addstr = role.Split(' ');
            string nick = addstr[2];
            string nick2 = nick[0].ToString().ToUpper();
            nick = nick.Remove(0, 1);
            nick = nick2 + nick;
            addstr[2] = nick;
            string html2 = await client.GetStringAsync("https://www.wowprogress.com/character/" + addstr[0] + "/" + addstr[1] + "/" + addstr[2]);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html2);
            int count = 0;
            await m_Service.RemoveRoleUser(Context.Guild, Context.User, "new raider");
            for (int i = 1; i <= 8; i++)
            {
                await m_Service.RemoveRoleUser(Context.Guild, Context.User, "Мифик " + i + "/8 ВД");
            }
            if (SetScoreHelper(document, "Abyssal Commander Sivara Mythic")) count++;
            if (SetScoreHelper(document, "Blackwater Behemoth Mythic")) count++;
            if (SetScoreHelper(document, "Radiance of Azshara Mythic")) count++;
            if (SetScoreHelper(document, "Lady Ashvane Mythic")) count++;
            if (SetScoreHelper(document, "Orgozoa Mythic")) count++;
            if (SetScoreHelper(document, "The Queen's Court Mythic")) count++;
            if (SetScoreHelper(document, "Za'qul, Harbinger of Ny'alotha Mythic")) count++;
            if (SetScoreHelper(document, "Queen Azshara Mythic")) count++;
            if (count == 0)
            {
                await m_Service.AddRoleUser(Context.Guild, Context.User, "new raider");
            }
            await m_Service.AddRoleUser(Context.Guild, Context.User, "Мифик " + count + "/8 ВД");
        }

        static bool SetScoreHelper(HtmlDocument htmlDocument, string BossName)
        {
            var nodes = htmlDocument.DocumentNode
                .Descendants()
                .Where(node => //node.Attributes.Contains("href") &&
                           node.InnerText.Trim().Contains(BossName))
            .Select(node => new
            {
                Text = node.InnerText.Trim(),
                Link = node.GetAttributeValue("href", "").Trim()
            });
            foreach (var htmlNode in nodes)
            {
                return true;
            }
            return false;
        }
    }
}
