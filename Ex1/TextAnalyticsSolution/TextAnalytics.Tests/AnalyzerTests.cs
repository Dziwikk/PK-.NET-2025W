using NUnit.Framework;
using TextAnalytics.Core;

namespace TextAnalytics.Tests;

public class AnalyzerTests
{
    private TextAnalyzer _analyzer = null!;

    [SetUp]
    public void Setup()
    {
        _analyzer = new TextAnalyzer();
    }

    [Test]
    public void Analyze_Empty_ReturnsZeros()
    {
        var s = _analyzer.Analyze("");
        Assert.That(s.WordCount, Is.EqualTo(0));
        Assert.That(s.SentenceCount, Is.EqualTo(0));
        Assert.That(s.CharactersWithSpaces, Is.EqualTo(0));
    }

    [Test]
    public void Words_AreTokenizedAcrossPunctuation()
    {
        var text = "Ala, ma! kota? Ala ma psa.";
        Assert.That(_analyzer.CountWords(text), Is.EqualTo(6));
        Assert.That(_analyzer.CountUniqueWords(text), Is.EqualTo(4)); // ala, ma, kota, psa

        var mcw = _analyzer.FindMostCommonWord(text).ToLowerInvariant();
        Assert.That(new[] { "ma", "ala" }, Does.Contain(mcw));
    }


    [Test]
    public void Sentences_CountsByTerminators()
    {
        var text = "To jest zdanie pierwsze. A to drugie! Czy trzecie?";
        Assert.That(_analyzer.CountSentences(text), Is.EqualTo(3));
        Assert.That(_analyzer.AverageWordsPerSentence(text), Is.GreaterThan(0));
    }

    [Test]
    public void LongestShortestWord_Works()
    {
        var text = "kot kotek koteczka";
        Assert.That(_analyzer.MaxWord(text), Is.EqualTo("koteczka"));
        Assert.That(_analyzer.MinWord(text), Is.EqualTo("kot"));
    }

    [Test]
    public void Characters_Counters_Work()
    {
        var txt = "Abc 123,!";
        Assert.That(_analyzer.CountCharacters(txt), Is.EqualTo(9));
        Assert.That(_analyzer.CountCharacters(txt, includeSpaces:false), Is.EqualTo(8));
        Assert.That(_analyzer.CountLetters(txt), Is.EqualTo(3));
        Assert.That(_analyzer.CountDigits(txt), Is.EqualTo(3));
        Assert.That(_analyzer.CountPunctuation(txt), Is.EqualTo(2)); // ,  !
    }
}