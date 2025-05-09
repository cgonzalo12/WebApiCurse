﻿namespace Biblioteca.DTOs
{
    public class AutorFiltroDTO
    {
        public int Pagina { get; set; } = 1;
        public int RecordsPorPagina { get; set; } = 10;
        public PaginacionDTO PaginacionDTO
        {
            get
            {
                return new PaginacionDTO(Pagina, RecordsPorPagina);
            }
        
        }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public bool? TieneFoto { get; set; }
        public bool? TieneLibros { get; set; }
        public string? TituloLibro { get; set; }
        public bool IncluirLibros { get; set; }
        public string? CamposOrdenar { get; set; }
        public bool OrdenAscendete { get; set; }=true;
        
    }
}
