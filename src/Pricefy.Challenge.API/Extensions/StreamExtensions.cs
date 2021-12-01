using System.IO;

namespace Pricefy.Challenge.API.Extensions
{
    public static class StreamExtensions
    {
        public static StreamReader ToStreamReader(this Stream stream)
            => new StreamReader(stream);
    }
}