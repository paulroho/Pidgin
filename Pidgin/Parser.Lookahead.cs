using System;
using Pidgin.ParseStates;

namespace Pidgin
{
    public static partial class Parser
    {
        /// <summary>
        /// If <paramref name="parser"/> succeeds, <code>Lookahead(parser)</code> backtracks,
        /// behaving as if <paramref name="parser"/> had not consumed any input.
        /// No backtracking is performed upon failure.
        /// </summary>
        /// <param name="parser">The parser to look ahead with</param>
        /// <returns>A parser which rewinds the input stream if <paramref name="parser"/> succeeds.</returns>
        public static Parser<TToken, T> Lookahead<TToken, T>(Parser<TToken, T> parser)
            => new LookaheadParser<TToken, T>(parser);

        private class LookaheadParser<TToken, T> : Parser<TToken, T>
        {
            private readonly Parser<TToken, T> _parser;

            public LookaheadParser(Parser<TToken, T> parser) : base(parser.Expected)
            {
                _parser = parser;
            }

            internal override Result<TToken, T> Parse(IParseState<TToken> state)
            {
                state.PushBookmark();

                var result = _parser.Parse(state);

                if (result.Success)
                {
                    state.Rewind();
                    return Result.Success<TToken, T>(result.GetValueOrDefault(), false);
                }
                state.PopBookmark();
                return result;
            }
        }
    }
}