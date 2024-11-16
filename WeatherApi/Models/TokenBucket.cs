using System.Collections.Generic;

namespace api.Models
{
    public class TokenBucket
    {
        public List<Token> Tokens { get; set; }

        public TokenBucket(List<string> keys, int capacity)
        {
            Tokens = new List<Token>();
            foreach (var key in keys)
            {
                Tokens.Add(new Token(key, capacity));
            }
        }
    }
}
