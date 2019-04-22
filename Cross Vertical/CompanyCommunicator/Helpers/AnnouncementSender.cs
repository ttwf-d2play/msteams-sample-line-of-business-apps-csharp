// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
using CrossVertical.Announcement.Helper;
using CrossVertical.Announcement.Repository;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrossVertical.Announcement.Helpers
{
    public class AnnouncementSender
    {
        public string AnnouncementId { get; set; }

        public async Task Execute()
        {
            var campaign = await Cache.Announcements.GetItemAsync(AnnouncementId);
            if (campaign == null)
                return;

            await SendAnnouncement(campaign);
        }

        public static async Task SendAnnouncement(Models.Campaign campaign)
        {
            var card = campaign.GetPreviewCard().ToAttachment();

            int successCount = 0;
            int failureCount = 0;
            int duplicateUsers = 0;
            List<string> notifiedUsers = new List<string>();
            var ownerId = (await Cache.Users.GetItemAsync(campaign.OwnerId)).BotConversationId;
            foreach (var group in campaign.Recipients.Groups)
            {
                foreach (var recipient in group.Users)
                {
                    var user = await Cache.Users.GetItemAsync(recipient.Id);
                    if (user == null)
                    {
                        recipient.FailureMessage = "App not installed";
                        failureCount++;
                        await ProactiveMessageHelper.SendNotification(campaign.Recipients.ServiceUrl, campaign.Recipients.TenantId, ownerId, "App not installed by " + recipient.Id, null);
                    }
                    else
                    {
                        if (notifiedUsers.Contains(recipient.Id))
                        {
                            recipient.FailureMessage = "Duplicated. Message already sent.";
                            duplicateUsers++;
                            continue;
                        }

                        var campaignCard = CardHelper.GetCardWithAcknowledgementDetails(card, campaign.Id, user.Id, group.GroupId);
                        var response = await ProactiveMessageHelper.SendNotification(campaign.Recipients.ServiceUrl, campaign.Recipients.TenantId, user.BotConversationId, null, card);
                        if (response.IsSuccessful)
                        {
                            recipient.MessageId = response.MessageId;
                            successCount++;
                            notifiedUsers.Add(recipient.Id);
                        }
                        else
                        {
                            recipient.FailureMessage = response.FailureMessage;
                            failureCount++;
                            await ProactiveMessageHelper.SendNotification(campaign.Recipients.ServiceUrl, campaign.Recipients.TenantId, ownerId, $"User: {user.Name}. Error: " + recipient.FailureMessage,  null);
                        }
                    }
                }
            }

            var botAccount = new ChannelAccount(ApplicationSettings.AppId);
            foreach (var recipient in campaign.Recipients.Channels)
            {
                var team = await Cache.Teams.GetItemAsync(recipient.TeamId);
                if (team == null)
                {
                    recipient.Channel.FailureMessage = "App not installed";
                    failureCount++;
                    await ProactiveMessageHelper.SendNotification(campaign.Recipients.ServiceUrl, campaign.Recipients.TenantId, ownerId, "App not installed in team: " + recipient.TeamId, null);
                }
                else
                {
                    var campaignCard = CardHelper.GetCardWithoutAcknowledgementAction(card);

                    if (notifiedUsers.Contains(recipient.Channel.Id))
                    {
                        recipient.Channel.FailureMessage = "Duplicated. Message already sent.";
                        duplicateUsers++;
                        continue;
                    }

                    var response = await ProactiveMessageHelper.SendChannelNotification(botAccount, campaign.Recipients.ServiceUrl, recipient.Channel.Id, null, card);
                    if (response.IsSuccessful)
                    {
                        recipient.Channel.MessageId = response.MessageId;
                        successCount++;
                        notifiedUsers.Add(recipient.Channel.Id);
                    }
                    else
                    {
                        recipient.Channel.FailureMessage = response.FailureMessage;
                        failureCount++;
                        await ProactiveMessageHelper.SendNotification(campaign.Recipients.ServiceUrl, campaign.Recipients.TenantId, ownerId, $"Team: {team.Name}. Error: " + recipient.Channel.FailureMessage, null); ;
                    }
                }
            }

            await ProactiveMessageHelper.SendNotification(campaign.Recipients.ServiceUrl, campaign.Recipients.TenantId, ownerId,
                                        $"Process completed. Successful: {successCount}. Failure: {failureCount}. Duplicate: {duplicateUsers}",  null); 

            campaign.Status = Models.Status.Sent;
            await Cache.Announcements.AddOrUpdateItemAsync(campaign.Id, campaign);
        }
    }
}