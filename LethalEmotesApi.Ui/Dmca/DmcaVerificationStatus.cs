using System;

namespace LethalEmotesApi.Ui.Dmca;

public enum DmcaVerificationStatus
{
    Unknown,
    Verified,
    NonDmcaCompliant,
    Ignore
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
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
    
    public static string Name(this DmcaVerificationStatus status)
    {
        return status switch
        {
            DmcaVerificationStatus.Unknown => "Unknown",
            DmcaVerificationStatus.Verified => "Verified",
            DmcaVerificationStatus.NonDmcaCompliant => "Not Dmca Compliant",
            DmcaVerificationStatus.Ignore => "Ignore",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}