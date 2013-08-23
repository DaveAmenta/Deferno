using System;
using System.Drawing;
using System.Text.RegularExpressions;

public class Colorizer
{
    static char COLOR = '\x03';
    static char BOLD = '\x02';
    static char STOP = '\x0F';
    static char UNDERLINE = '\x1F';

    static string COLORENT = "span";
    static string BOLDENT = "strong";
    static string UNDENT = "u";

    static string BOLD_RX = BOLD + "(.*?)(" + BOLD + "|" + STOP + "|$)";
    static string UNDERLINE_RX = UNDERLINE + "(.*?)(" + UNDERLINE + "|" + STOP + "|$)";
    static string COLOR_RX = COLOR + "(1[0-5]|[0-9])(?:,(1[0-5]|[0-9]))?(.*?)(?=" + COLOR + "|" + STOP + "|$)";


    /// <summary>
    /// Converts a set of IRC color codes to their corresponding HTML values.
    /// </summary>
    /// <param name="toConvert">The data to convert to HTML.</param>
    /// <returns>The unescaped HTML output.</returns>
    public static string ParseStyleCodes(string toConvert)
    {
        // Bold
        toConvert = Regex.Replace(toConvert, BOLD_RX, "<" + BOLDENT + ">$1</" + BOLDENT + ">");

        // Underline
        toConvert = Regex.Replace(toConvert, UNDERLINE_RX, "<" + UNDENT + ">$1</" + UNDENT + ">");

        // Color
        toConvert = Regex.Replace(toConvert, COLOR_RX, new MatchEvaluator(Colorizer.ColorReplacer));

        // Remove the extra stop and color codes
        toConvert = Regex.Replace(toConvert, STOP.ToString(), "");
        toConvert = Regex.Replace(toConvert, COLOR.ToString(), "");

        return toConvert;
    }

    /// <summary>
    /// Takes the Regex color results and formats them accordingly.
    /// </summary>
    /// <param name="match">The current regex token.</param>
    /// <returns>The formatted string.</returns>
    public static string ColorReplacer(Match match)
    {
        string backgroundAdd = "";

        if (!string.IsNullOrEmpty(match.Groups[2].Value))
        {
            backgroundAdd = "; background-color: " + ColorTranslator.ToHtml(Colorizer.GetColorValues()[int.Parse(match.Groups[2].Value)]) + ";";
        }
        string color = ColorTranslator.ToHtml(Colorizer.GetColorValues()[int.Parse(match.Groups[1].Value)]);

        return String.Format("<" + COLORENT + " style=\"color: {0}{1}\">{2}</" + COLORENT + ">", color, backgroundAdd, match.Groups[3].Value);
    }


    // DBG:Should be replaced with a proper color picker.
    public static Color[] GetColorValues()
    {
        return new Color[]
        {
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(0, 0, 128),
            Color.FromArgb(0, 128, 0),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(128, 0, 0), // 5
            Color.FromArgb(128, 0, 128),
            Color.FromArgb(255, 128, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(0, 128, 128), // 10
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(128, 128, 128),
            Color.FromArgb(210, 210, 210)
        };
    }
}