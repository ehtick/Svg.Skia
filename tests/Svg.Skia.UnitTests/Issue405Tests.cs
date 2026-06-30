#pragma warning disable CS0618 // Typeface and FakeBoldText are deprecated on SKPaint; shim keeps the legacy surface for compatibility

using ShimSkiaSharp;
using Svg.Skia;
using Xunit;

namespace Svg.Skia.UnitTests;

public class Issue405Tests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SansSerifBold_ResolvesSingleTypefaceSpan(bool enableSvgFonts)
    {
        var settings = new SKSvgSettings { EnableSvgFonts = enableSvgFonts };
        var assetLoader = new SkiaSvgAssetLoader(new SkiaModel(settings));
        var paint = new SKPaint
        {
            Typeface = SKTypeface.FromFamilyName(
                "sans-serif",
                SKFontStyleWeight.Bold,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright)
        };

        var spans = assetLoader.FindTypefaces("Bold Text 20px", paint);

        Assert.Single(spans);
        var span = spans[0];
        Assert.NotNull(span.Typeface);
        Assert.True(span.Typeface!.FontWeight >= SKFontStyleWeight.SemiBold,
            "Expected resolved typeface to be semi-bold or heavier.");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FakeBoldMatchesDesiredWeight(bool enableSvgFonts)
    {
        var settings = new SKSvgSettings { EnableSvgFonts = enableSvgFonts };
        var model = new SkiaModel(settings);
        var paint = new SKPaint
        {
            Typeface = SKTypeface.FromFamilyName(
                "sans-serif",
                SKFontStyleWeight.ExtraBlack,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright)
        };

        using var localFont = model.ToSKFont(paint);
        Assert.NotNull(localFont);

        var desiredWeight = (int)SkiaSharp.SKFontStyleWeight.ExtraBlack;
        var actualWeight = localFont!.Typeface?.FontWeight ?? 0;
        var shouldFakeBold = actualWeight < desiredWeight;

        Assert.Equal(shouldFakeBold, localFont.Embolden);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void WhitespaceStaysAttachedToResolvedTypefaceSpan(bool enableSvgFonts)
    {
        var settings = new SKSvgSettings { EnableSvgFonts = enableSvgFonts };
        var assetLoader = new SkiaSvgAssetLoader(new SkiaModel(settings));
        var paint = new SKPaint();

        var spans = assetLoader.FindTypefaces("ښ ښښښ", paint);

        var span = Assert.Single(spans);
        Assert.Equal("ښ ښښښ", span.Text);
        Assert.NotNull(span.Typeface);
    }
}

#pragma warning restore CS0618
