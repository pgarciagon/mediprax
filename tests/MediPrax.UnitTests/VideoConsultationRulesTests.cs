using MediPrax.Application.Services;

namespace MediPrax.UnitTests;

public class VideoConsultationRulesTests
{
    [Fact]
    public void FirstSprechstunde_NotAllowedAsVideo()
    {
        Assert.False(VideoConsultationRules.IsFirstSprechstundeAllowedAsVideo(0));
    }

    [Fact]
    public void SecondSprechstunde_AllowedAsVideo()
    {
        Assert.True(VideoConsultationRules.IsFirstSprechstundeAllowedAsVideo(1));
    }

    [Fact]
    public void MinimumInPersonTime_NotMet_WhenLessThan50()
    {
        Assert.False(VideoConsultationRules.HasMinimumInPersonTime(49));
    }

    [Fact]
    public void MinimumInPersonTime_Met_WhenExactly50()
    {
        Assert.True(VideoConsultationRules.HasMinimumInPersonTime(50));
    }

    [Theory]
    [InlineData("35100", true)]
    [InlineData("21220", true)]
    [InlineData("16220", true)]
    [InlineData("35140", true)]
    [InlineData("16210", false)]
    [InlineData("99999", false)]
    public void IsVideoEligible_ReturnsCorrectResult(string gopCode, bool expected)
    {
        Assert.Equal(expected, VideoConsultationRules.IsVideoEligible(gopCode));
    }

    [Fact]
    public void GetVideoVariantCode_ReturnsWithSuffix_ForEligible()
    {
        Assert.Equal("35100V", VideoConsultationRules.GetVideoVariantCode("35100"));
    }

    [Fact]
    public void GetVideoVariantCode_ReturnsNull_ForIneligible()
    {
        Assert.Null(VideoConsultationRules.GetVideoVariantCode("16210"));
    }

    [Fact]
    public void VideoRatio_Acceptable_When75PercentOrLess()
    {
        Assert.True(VideoConsultationRules.IsVideoRatioAcceptable(3, 4)); // 75%
        Assert.True(VideoConsultationRules.IsVideoRatioAcceptable(1, 4)); // 25%
    }

    [Fact]
    public void VideoRatio_NotAcceptable_WhenOver75Percent()
    {
        Assert.False(VideoConsultationRules.IsVideoRatioAcceptable(4, 5)); // 80%
    }

    [Fact]
    public void VideoRatio_Acceptable_WhenNoSessions()
    {
        Assert.True(VideoConsultationRules.IsVideoRatioAcceptable(0, 0));
    }
}
