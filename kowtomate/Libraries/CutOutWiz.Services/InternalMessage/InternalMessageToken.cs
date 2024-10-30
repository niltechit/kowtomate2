

namespace CutOutWiz.Services.InternalMessage
{
    public sealed class InternalMessageToken
    {
        public InternalMessageToken(string key, string value) :
        this(key, value, false)
        {
        }

        public InternalMessageToken(string key, string value, bool neverHtmlEncoded)
        {
            Key = key;
            Value = value;
            NeverHtmlEncoded = neverHtmlEncoded;
        }

        /// <summary>
        /// Token key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Token value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Indicates whether this token should not be HTML encoded
        /// </summary>
        public bool NeverHtmlEncoded { get; }

        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}
