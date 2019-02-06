using AdaptiveCards;
using Microsoft.Bot.Connector;

namespace CrossVertical.Announcement.Helpers
{
    /// <summary>
    ///  Adaptive card extension classes.
    /// </summary>
    public static class AdaptiveCardExtensions
    {
        public static Attachment ToAttachment(this AdaptiveCard card)
        {
            return new Attachment
            {
                Content = card,
                ContentType = AdaptiveCard.ContentType
            };
        }
    }

    public static class TaskModuleUIConstants
    {
        public static UIConstants CreateNewAnnouncement { get; set; } =
            new UIConstants(1200, 600, "Create New Announcement", "adaptivecard", "Adaptive Card");
    }
    public class UIConstants
    {
        public UIConstants(int width, int height, string title, string id, string buttonTitle)
        {
            Width = width;
            Height = height;
            Title = title;
            Id = id;
            ButtonTitle = buttonTitle;
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public string Title { get; set; }
        public string ButtonTitle { get; set; }
        public string Id { get; set; }
    }
}