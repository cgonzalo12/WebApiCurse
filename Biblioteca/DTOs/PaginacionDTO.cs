namespace Biblioteca.DTOs
{
    public class PaginacionDTO
    {
        private const int CantidadMaximaRecordsPorPagina = 50;

        private int pagina = 1;
        public int Pagina
        {
            get => pagina;
            set => pagina = (value < 1) ? 1 : value;
        }

        private int recordsPorPagina = 10;
        public int RecordsPorPagina
        {
            get => recordsPorPagina;
            set => recordsPorPagina = Math.Clamp(value, 1, CantidadMaximaRecordsPorPagina);
        }
    }
}
