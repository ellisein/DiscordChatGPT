using Discord;
using Discord.Interactions;

namespace DiscordChatGPT.Models.Modals;

public class ImageGenerationModal : IModal
{
    public string Title => "Image Generation";
    
    [InputLabel("Aspect Ratio (square | wide | portrait)")]
    [ModalTextInput("aspect_ratio", placeholder: "square")]
    public string AspectRatio { get; set; } = "square";

    [InputLabel("Quality (standard | hd)")]
    [ModalTextInput("quality", placeholder: "standard")]
    public string Quality { get; set; } = "standard";

    [InputLabel("Prompt")]
    [ModalTextInput("prompt", TextInputStyle.Paragraph, maxLength: 2000)]
    public string Prompt { get; set; } = "";
}