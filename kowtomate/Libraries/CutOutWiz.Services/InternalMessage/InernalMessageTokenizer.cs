using System.Web;

namespace CutOutWiz.Services.InternalMessage
{
    public class InernalMessageTokenizer : IInernalMessageTokenizer
    {
        private readonly StringComparison _stringComparison;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="settings">Message templates settings</param>
        public InernalMessageTokenizer(InternalMessageSettings settings = null)
        {
            _stringComparison = settings == null || settings.CaseInvariantReplacement
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal;
        }

        /// <summary>
        /// Replace all of the token key occurences inside the specified template text with 
        /// corresponded token values
        /// </summary>
        /// <param name="template">The template with token keys inside</param>
        /// <param name="tokens">The sequence of tokens to use</param>
        /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
        /// <returns>Text with all token keys replaces by token value</returns>
        public string Replace(string template, IEnumerable<InternalMessageToken> tokens, bool htmlEncode)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentNullException(nameof(template));

            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            foreach (var token in tokens)
            {
                var tokenValue = token.Value;

                //do not encode URLs
                if (htmlEncode && !token.NeverHtmlEncoded)
                    tokenValue = HttpUtility.HtmlEncode(tokenValue);

                template = Replace(template, @"{" + token.Key + "}", tokenValue);
            }
            return template;
        }

        private string Replace(string original, string pattern, string replacement)
        {
            if (_stringComparison == StringComparison.Ordinal)
            {
                return original.Replace(pattern, replacement);
            }

            int position0;
            int position1;

            var count = position0 = 0;

            var inc = original.Length / pattern.Length * (replacement.Length - pattern.Length);
            var chars = new char[original.Length + Math.Max(0, inc)];

            while ((position1 = original.IndexOf(pattern, position0, _stringComparison)) != -1)
            {
                for (var i = position0; i < position1; ++i)
                    chars[count++] = original[i];

                for (var i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0) return original;

            for (var i = position0; i < original.Length; ++i)
                chars[count++] = original[i];

            return new string(chars, 0, count);

        }
    }
}
