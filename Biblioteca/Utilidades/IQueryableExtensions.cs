﻿using Biblioteca.DTOs;
using System.Runtime.CompilerServices;

namespace Biblioteca.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                    .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina)
                    .Take(paginacionDTO.RecordsPorPagina);
        }
    }
}
