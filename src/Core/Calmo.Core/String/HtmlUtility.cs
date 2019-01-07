using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace System
{
    public class HtmlUtility
    {
        private enum UnicodeDecodingConformance
        {
            Auto,
            Strict,
            Compat,
            Loose
        }

        private static readonly char[] HtmlEntityEndingChars = { ';', '&' };
        private static readonly UnicodeDecodingConformance _htmlDecodeConformance = UnicodeDecodingConformance.Auto;

        public static string HtmlDecode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (!HtmlUtility.StringRequiresHtmlDecoding(value))
                return value;

            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            HtmlUtility.HtmlDecode(value, stringWriter);

            return stringWriter.ToString();
        }

        private static bool StringRequiresHtmlDecoding(string s)
        {
            if (HtmlUtility._htmlDecodeConformance == UnicodeDecodingConformance.Compat)
                return s.IndexOf('&') >= 0;

            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '&' || char.IsSurrogate(c))
                    return true;
            }

            return false;
        }

        private static void ConvertSmpToUtf16(uint smpChar, out char leadingSurrogate, out char trailingSurrogate)
        {
            var num = (int)(smpChar - 65536u);

            leadingSurrogate = (char)(num / 1024 + 55296);
            trailingSurrogate = (char)(num % 1024 + 56320);
        }

        public static void HtmlDecode(string value, TextWriter output)
        {
            if (value == null)
                return;

            if (output == null)
                throw new ArgumentNullException("output");

            if (!HtmlUtility.StringRequiresHtmlDecoding(value))
            {
                output.Write(value);
                return;
            }

            var length = value.Length;
            var i = 0;
            while (i < length)
            {
                var c = value[i];
                if (c != '&')
                    goto IL_1B6;

                var num = value.IndexOfAny(HtmlUtility.HtmlEntityEndingChars, i + 1);
                if (num <= 0 || value[num] != ';')
                    goto IL_1B6;

                var text = value.Substring(i + 1, num - i - 1);
                if (text.Length > 1 && text[0] == '#')
                {
                    uint num2;
                    bool flag;
                    if (text[1] == 'x' || text[1] == 'X')
                        flag = uint.TryParse(text.Substring(2), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out num2);
                    else
                        flag = uint.TryParse(text.Substring(1), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2);

                    if (flag)
                    {
                        switch (HtmlUtility._htmlDecodeConformance)
                        {
                            case UnicodeDecodingConformance.Strict:
                                flag = (num2 < 55296u || (57343u < num2 && num2 <= 1114111u));
                                break;
                            case UnicodeDecodingConformance.Compat:
                                flag = (0u < num2 && num2 <= 65535u);
                                break;
                            case UnicodeDecodingConformance.Loose:
                                flag = (num2 <= 1114111u);
                                break;
                            default:
                                flag = false;
                                break;
                        }
                    }

                    if (!flag)
                        goto IL_1B6;

                    if (num2 <= 65535u)
                        output.Write((char) num2);
                    else
                    {
                        char value2;
                        char value3;
                        HtmlUtility.ConvertSmpToUtf16(num2, out value2, out value3);
                        output.Write(value2);
                        output.Write(value3);
                    }

                    i = num;
                }
                else
                {
                    i = num;
                    var c2 = HtmlUtility.HtmlEntities.Lookup(text);
                    if (c2 != '\0')
                    {
                        c = c2;
                        goto IL_1B6;
                    }

                    output.Write('&');
                    output.Write(text);
                    output.Write(';');
                }

                IL_1BD:
                i++;
                continue;
                IL_1B6:
                output.Write(c);
                goto IL_1BD;
            }
        }


        private static class HtmlEntities
        {
            private static string[] _entitiesList = {
                "\"-quot",
                "&-amp",
                "'-apos",
                "<-lt",
                ">-gt",
                "\u00a0-nbsp",
                "¡-iexcl",
                "¢-cent",
                "£-pound",
                "¤-curren",
                "¥-yen",
                "¦-brvbar",
                "§-sect",
                "¨-uml",
                "©-copy",
                "ª-ordf",
                "«-laquo",
                "¬-not",
                "­-shy",
                "®-reg",
                "¯-macr",
                "°-deg",
                "±-plusmn",
                "²-sup2",
                "³-sup3",
                "´-acute",
                "µ-micro",
                "¶-para",
                "·-middot",
                "¸-cedil",
                "¹-sup1",
                "º-ordm",
                "»-raquo",
                "¼-frac14",
                "½-frac12",
                "¾-frac34",
                "¿-iquest",
                "À-Agrave",
                "Á-Aacute",
                "Â-Acirc",
                "Ã-Atilde",
                "Ä-Auml",
                "Å-Aring",
                "Æ-AElig",
                "Ç-Ccedil",
                "È-Egrave",
                "É-Eacute",
                "Ê-Ecirc",
                "Ë-Euml",
                "Ì-Igrave",
                "Í-Iacute",
                "Î-Icirc",
                "Ï-Iuml",
                "Ð-ETH",
                "Ñ-Ntilde",
                "Ò-Ograve",
                "Ó-Oacute",
                "Ô-Ocirc",
                "Õ-Otilde",
                "Ö-Ouml",
                "×-times",
                "Ø-Oslash",
                "Ù-Ugrave",
                "Ú-Uacute",
                "Û-Ucirc",
                "Ü-Uuml",
                "Ý-Yacute",
                "Þ-THORN",
                "ß-szlig",
                "à-agrave",
                "á-aacute",
                "â-acirc",
                "ã-atilde",
                "ä-auml",
                "å-aring",
                "æ-aelig",
                "ç-ccedil",
                "è-egrave",
                "é-eacute",
                "ê-ecirc",
                "ë-euml",
                "ì-igrave",
                "í-iacute",
                "î-icirc",
                "ï-iuml",
                "ð-eth",
                "ñ-ntilde",
                "ò-ograve",
                "ó-oacute",
                "ô-ocirc",
                "õ-otilde",
                "ö-ouml",
                "÷-divide",
                "ø-oslash",
                "ù-ugrave",
                "ú-uacute",
                "û-ucirc",
                "ü-uuml",
                "ý-yacute",
                "þ-thorn",
                "ÿ-yuml",
                "Œ-OElig",
                "œ-oelig",
                "Š-Scaron",
                "š-scaron",
                "Ÿ-Yuml",
                "ƒ-fnof",
                "ˆ-circ",
                "˜-tilde",
                "Α-Alpha",
                "Β-Beta",
                "Γ-Gamma",
                "Δ-Delta",
                "Ε-Epsilon",
                "Ζ-Zeta",
                "Η-Eta",
                "Θ-Theta",
                "Ι-Iota",
                "Κ-Kappa",
                "Λ-Lambda",
                "Μ-Mu",
                "Ν-Nu",
                "Ξ-Xi",
                "Ο-Omicron",
                "Π-Pi",
                "Ρ-Rho",
                "Σ-Sigma",
                "Τ-Tau",
                "Υ-Upsilon",
                "Φ-Phi",
                "Χ-Chi",
                "Ψ-Psi",
                "Ω-Omega",
                "α-alpha",
                "β-beta",
                "γ-gamma",
                "δ-delta",
                "ε-epsilon",
                "ζ-zeta",
                "η-eta",
                "θ-theta",
                "ι-iota",
                "κ-kappa",
                "λ-lambda",
                "μ-mu",
                "ν-nu",
                "ξ-xi",
                "ο-omicron",
                "π-pi",
                "ρ-rho",
                "ς-sigmaf",
                "σ-sigma",
                "τ-tau",
                "υ-upsilon",
                "φ-phi",
                "χ-chi",
                "ψ-psi",
                "ω-omega",
                "ϑ-thetasym",
                "ϒ-upsih",
                "ϖ-piv",
                "\u2002-ensp",
                "\u2003-emsp",
                "\u2009-thinsp",
                "‌-zwnj",
                "‍-zwj",
                "‎-lrm",
                "‏-rlm",
                "–-ndash",
                "—-mdash",
                "‘-lsquo",
                "’-rsquo",
                "‚-sbquo",
                "“-ldquo",
                "”-rdquo",
                "„-bdquo",
                "†-dagger",
                "‡-Dagger",
                "•-bull",
                "…-hellip",
                "‰-permil",
                "′-prime",
                "″-Prime",
                "‹-lsaquo",
                "›-rsaquo",
                "‾-oline",
                "⁄-frasl",
                "€-euro",
                "ℑ-image",
                "℘-weierp",
                "ℜ-real",
                "™-trade",
                "ℵ-alefsym",
                "←-larr",
                "↑-uarr",
                "→-rarr",
                "↓-darr",
                "↔-harr",
                "↵-crarr",
                "⇐-lArr",
                "⇑-uArr",
                "⇒-rArr",
                "⇓-dArr",
                "⇔-hArr",
                "∀-forall",
                "∂-part",
                "∃-exist",
                "∅-empty",
                "∇-nabla",
                "∈-isin",
                "∉-notin",
                "∋-ni",
                "∏-prod",
                "∑-sum",
                "−-minus",
                "∗-lowast",
                "√-radic",
                "∝-prop",
                "∞-infin",
                "∠-ang",
                "∧-and",
                "∨-or",
                "∩-cap",
                "∪-cup",
                "∫-int",
                "∴-there4",
                "∼-sim",
                "≅-cong",
                "≈-asymp",
                "≠-ne",
                "≡-equiv",
                "≤-le",
                "≥-ge",
                "⊂-sub",
                "⊃-sup",
                "⊄-nsub",
                "⊆-sube",
                "⊇-supe",
                "⊕-oplus",
                "⊗-otimes",
                "⊥-perp",
                "⋅-sdot",
                "⌈-lceil",
                "⌉-rceil",
                "⌊-lfloor",
                "⌋-rfloor",
                "〈-lang",
                "〉-rang",
                "◊-loz",
                "♠-spades",
                "♣-clubs",
                "♥-hearts",
                "♦-diams"
            };

            private static Dictionary<string, char> _lookupTable = HtmlUtility.HtmlEntities.GenerateLookupTable();
            private static Dictionary<string, char> GenerateLookupTable()
            {
                var dictionary = new Dictionary<string, char>(StringComparer.Ordinal);
                var entitiesList = HtmlUtility.HtmlEntities._entitiesList;

                for (int i = 0; i < entitiesList.Length; i++)
                {
                    var text = entitiesList[i];
                    dictionary.Add(text.Substring(2), text[0]);
                }
                return dictionary;
            }

            public static char Lookup(string entity)
            {
                char result;
                HtmlUtility.HtmlEntities._lookupTable.TryGetValue(entity, out result);
                return result;
            }
        }
    }
}
