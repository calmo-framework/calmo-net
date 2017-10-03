using System.Linq;

namespace System.Collections.Generic
{
    public static class PagedListExtensions
    {
        public static PagedList<TResult> SelectPaged<TSource, TResult>(this PagedList<TSource> source, Func<TSource, TResult> selector)
        {
            var members = source.Select(selector);

            return new PagedList<TResult>(members, source.Page, source.TotalCount, source.PageSize);
        }

        public static PagedList<TResult, TPageIdentificator> SelectPaged<TSource, TPageIdentificator, TResult>(this PagedList<TSource, TPageIdentificator> source, Func<TSource, TResult> selector)
        {
            var members = source.Select(selector);

            return new PagedList<TResult, TPageIdentificator>(members, source.Page, source.TotalCount, source.PageSize, source.NextPage, source.PrevPage);
        }
    }
}