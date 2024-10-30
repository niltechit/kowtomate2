using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CutOutWiz.Services.MapperHelper
{
    public class MapperHelperService : IMapperHelperService
    {

        public async Task<List<TDestination>> MapToListAsync<TSource, TDestination>(IEnumerable<TSource> sourceList)
        {
            try
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TSource, TDestination>();
                });

                var mapper = configuration.CreateMapper();

                return await Task.Run(() => mapper.Map<List<TDestination>>(sourceList));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // For example: logger.LogError(ex, "Error occurred while mapping lists");
                throw new Exception("An error occurred while mapping lists", ex);
            }
        }

        public async Task<TDestination> MapToSingleAsync<TSource, TDestination>(TSource source)
        {
            try
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TSource, TDestination>();
                });

                var mapper = configuration.CreateMapper();

                return await Task.Run(() => mapper.Map<TDestination>(source));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // For example: logger.LogError(ex, "Error occurred while mapping lists");
                throw new Exception("An error occurred while mapping lists", ex);
            }
        }
    }
}
