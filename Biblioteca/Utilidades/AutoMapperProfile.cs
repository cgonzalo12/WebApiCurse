﻿using AutoMapper;
using Biblioteca.DTOs;
using Biblioteca.Entidades;

namespace Biblioteca.Utilidades
{
    public class AutoMapperProfile :Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Autor, AutorDTO>()
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(autor => MappearNombreYApellidoAutor(autor)));

            CreateMap<Autor, AutorConLibrosDTO>()
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(autor => MappearNombreYApellidoAutor(autor)));

            CreateMap<AutorCreacionDTO,Autor>();

            CreateMap<Autor, AutorPatchDTO>().ReverseMap();
            CreateMap<AutorLibro, LibroDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.LibroId))
                .ForMember(dto => dto.Titulo, config => config.MapFrom(ent => ent.Libro!.Titulo));


            CreateMap<Libro, LibroDTO>();

            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(ent=>ent.Autores,config=>
                    config.MapFrom(dto=>dto.AutoresIds.Select(id=>new AutorLibro { AutorId=id})));
            CreateMap<Libro, LibroConAutoresDTO>();
            CreateMap<AutorLibro, AutorDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.AutorId))
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(ent => MappearNombreYApellidoAutor(ent.Autor!)));

            CreateMap<LibroCreacionDTO, AutorLibro>()
                .ForMember(ent => ent.Libro, config => config.MapFrom(ent => new Libro { Titulo = ent.Titulo }));

                
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario,ComentarioCreacionDTO>();
            CreateMap<ComentarioPatchDTO,Comentario>().ReverseMap();
            CreateMap<Comentario, ComentarioDTO>()
                .ForMember(dto => dto.UsuarioEmail, config => config.MapFrom(ent => ent.Usuario!.Email));

            CreateMap<Usuario, UsuarioDTO>();

        }
        private string MappearNombreYApellidoAutor(Autor autor) => $"{autor.Nombres} {autor.Apellidos}";
    }
}
