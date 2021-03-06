﻿// <copyright file="GuildKickPlayerAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild
{
    using MUnique.OpenMU.Interfaces;
    using Views;

    /// <summary>
    /// Action to kick a player out of a guild.
    /// </summary>
    public class GuildKickPlayerAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(GuildKickPlayerAction));

        private readonly IGameServerContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildKickPlayerAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public GuildKickPlayerAction(IGameServerContext gameContext)
        {
            this.gameContext = gameContext;
        }

        /// <summary>
        /// Kicks the player out of the guild.
        /// </summary>
        /// <param name="guildMaster">The guild master.</param>
        /// <param name="nickname">The nickname.</param>
        /// <param name="securityCode">The security code.</param>
        public void KickPlayer(Player guildMaster, string nickname, string securityCode)
        {
            if (guildMaster.Account.SecurityCode != securityCode)
            {
                guildMaster.PlayerView.ShowMessage("Wrong Security Code.", MessageType.BlueNormal);
                Log.WarnFormat("Wrong Security Code: [{0}] <> [{1}], Player: {2}", securityCode, guildMaster.Account.SecurityCode, guildMaster.SelectedCharacter.Name);

                guildMaster.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.Failed);
                return;
            }

            var guildId = guildMaster.SelectedCharacter?.GuildMemberInfo?.Id;
            if (guildId.HasValue)
            {
                if (guildMaster.SelectedCharacter.GuildMemberInfo.Status == DataModel.Entities.GuildPosition.GuildMaster)
                {
                    guildMaster.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.Failed);
                }

                if (nickname == guildMaster.SelectedCharacter.Name)
                {
                    this.gameContext.GuildServer.DeleteGuild(guildId.Value);
                    this.gameContext.GuildCache.Invalidate(guildId.Value);
                    guildMaster.PlayerView.GuildView.GuildKickResult(GuildKickSuccess.GuildDisband);
                    return;
                }
                else
                {
                    this.gameContext.GuildServer.KickPlayer(guildId.Value, nickname);
                }
            }
        }
    }
}
