
namespace CutOutWiz.Services.MapperHelper
{
    public interface IMapperHelperService
    {
        /// <summary>
        /// Here Entity of list to list convert
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="sourceList"></param>
        /// <returns></returns>
        Task<List<TDestination>> MapToListAsync<TSource, TDestination>(IEnumerable<TSource> sourceList);
        Task<TDestination> MapToSingleAsync<TSource, TDestination>(TSource source);
    }
}
