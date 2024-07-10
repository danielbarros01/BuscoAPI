using BuscoAPI.DTOS;

namespace BuscoAPI.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.NumberRecordsPerPage)
                .Take(pagination.NumberRecordsPerPage); //selecciona los registros
        }
    }
}
