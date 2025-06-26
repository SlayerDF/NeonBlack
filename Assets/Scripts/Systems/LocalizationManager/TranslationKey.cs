using System;
using System.Linq;

namespace NeonBlack.Systems.LocalizationManager
{
    public struct TranslationKey : IEquatable<TranslationKey>
    {
        public readonly string RawText;
        public readonly object[] Args;

        public TranslationKey(string rawText, object[] args)
        {
            RawText = rawText;
            Args = args;
        }

        public bool Equals(TranslationKey other)
        {
            return RawText == other.RawText && Args.SequenceEqual(other.Args);
        }

        public override int GetHashCode()
        {
            var hash = RawText.GetHashCode();
            return Args.Aggregate(hash, (current, arg) => HashCode.Combine(current, arg?.GetHashCode() ?? 0));
        }
    }
}
