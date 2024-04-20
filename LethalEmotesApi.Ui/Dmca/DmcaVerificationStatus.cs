using System;

namespace LethalEmotesApi.Ui.Dmca;

public enum DmcaVerificationStatus
{
    Unknown,
    Verified,
    NonDmcaCompliant,
    Ignore,
    ThirdPartySettings
}

public static class DmcaVerificationStatusExtensions
{
    public static string AsColorTag(this DmcaVerificationStatus status)
    {
        return status switch
        {
            DmcaVerificationStatus.Unknown => "<color=#FFC600>",
            DmcaVerificationStatus.Verified => "<color=#00FF45>",
            DmcaVerificationStatus.NonDmcaCompliant => "<color=#FF1B00>",
            DmcaVerificationStatus.Ignore => "<color=#FFFFFF>",
            DmcaVerificationStatus.ThirdPartySettings => "<color=#66C3CC>",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
    
    public static string Name(this DmcaVerificationStatus status)
    {
        return status switch
        {
            DmcaVerificationStatus.Unknown => "Unknown",
            DmcaVerificationStatus.Verified => "Verified",
            DmcaVerificationStatus.NonDmcaCompliant => "Muted",
            DmcaVerificationStatus.Ignore => "Ignore",
            DmcaVerificationStatus.ThirdPartySettings => "Configure Separately",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
    
    public static string Tooltip(this DmcaVerificationStatus status)
    {
        return status switch
        {
            DmcaVerificationStatus.Unknown => "We haven't reviewed this emote pack yet. Use at your own risk.",
            DmcaVerificationStatus.Verified => "This emote pack correctly uses the DMCA options provided by LethalEmotesAPI. Make sure your DMCA options are configured correctly.",
            DmcaVerificationStatus.NonDmcaCompliant => "When reviewing this emote pack, we found it was non-compliant so it will be muted when DMCA settings are on.",
            DmcaVerificationStatus.Ignore => "Ignore",
            DmcaVerificationStatus.ThirdPartySettings => "This mod must be configured in it's own settings.",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static bool IsVerified(this DmcaVerificationStatus status)
    {
        return status is not DmcaVerificationStatus.Unknown and not DmcaVerificationStatus.ThirdPartySettings;
    }
}