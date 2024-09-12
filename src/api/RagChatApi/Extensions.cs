using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagChatApi;
public static class Extensions
{
    public static ValueTask<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource>? source, CancellationToken cancellationToken = default)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        else
        {
            return Core(source, cancellationToken);
        }

        static async ValueTask<List<TSource>> Core(IAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
        {
            List<TSource> list = [];

            await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                list.Add(item);
            }

            return list;
        }
    }
}
