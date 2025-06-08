using AutoMapper;
using Modulith.NewModule.Api.Dtos;
using Modulith.NewModule.Entities;
using System.Collections.Generic;

namespace Modulith.NewModule.Api;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<TodoItem, TodoItemDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Value))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Details.Description))
            .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.Details.DueDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsComplete ? "Complete" : "Pending"));
        CreateMap<TodoList, TodoListDto>();
    }
} 