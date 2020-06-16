using AutoMapper;
using sors.Authentication;
using sors.Data.Dto;
using sors.Data.Dto.Incidents;
using sors.Data.Dto.References;
using sors.Data.Entities;
using sors.Data.Entities.Incidents;
using sors.Data.Entities.Passports;
using sors.Data.Entities.References;
using System;
using System.Collections.Generic;

namespace sors.Helpers
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {

            CreateMap<Department, DepartmentForListDto>();
            CreateMap<DepartmentForListDto, Department>();
            CreateMap<DepartmentForCreateDto, Department>();
            CreateMap<DomainDepartment, DomainDepartmentForListDto>();
            CreateMap<DomainDepartmentForCreateDto, DomainDepartment>();
            CreateMap<AccountForCreateDto, Account>();
            CreateMap<Account, AccountForListDto>()
                .ConvertUsing<AccountTypeConverter>();
            CreateMap<Account, AccountForLoginDto>();
            CreateMap<Role, RoleForListDto>();
            CreateMap<AccountRole, RoleForListDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Role.Id))
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Role.Name));
            CreateMap<DraftForCreateDto, Draft>();
            CreateMap<Draft, DraftForDetailDto>();
            CreateMap<Draft, DraftForListDto>();
            CreateMap<Incident, IncidentForListDto>();
            CreateMap<Incident, IncidentForDetailDto>();
            CreateMap<IncidentForCreateDto, Incident>();
            CreateMap<Responsible, ResponsibleForListDto>();
            CreateMap<Responsible, ResponsibleForDetailDto>();
            CreateMap<ResponsibleForCreateDto, Responsible>();
            CreateMap<ResponsibleForListDto, Responsible>();
            CreateMap<IncidentDraft, DraftForDetailDto>()
                //.ConvertUsing<IncidentDraftTypeConverter>();
                //.ForMember(dest => dest, opts => opts.MapFrom(src => src.Draft));
                .ForMember(dest => dest.IncidentType, opts => opts.MapFrom(src => src.Draft.IncidentType))
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Draft.Id))
                .ForMember(dest => dest.Author, opts => opts.MapFrom(src => src.Draft.Author))
                .ForMember(dest => dest.DateCreate, opts => opts.MapFrom(src => src.Draft.DateCreate))
                .ForMember(dest => dest.Department, opts => opts.MapFrom(src => src.Draft.Department))
                .ForMember(dest => dest.Description1, opts => opts.MapFrom(src => src.Draft.Description1))
                .ForMember(dest => dest.Description2, opts => opts.MapFrom(src => src.Draft.Description2))
                .ForMember(dest => dest.Description3, opts => opts.MapFrom(src => src.Draft.Description3))
                .ForMember(dest => dest.Description4, opts => opts.MapFrom(src => src.Draft.Description4))
                .ForMember(dest => dest.Description5, opts => opts.MapFrom(src => src.Draft.Description5))
                .ForMember(dest => dest.Props, opts => opts.MapFrom(src => src.Draft.Props))
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Draft.Status));
            CreateMap<ResponsibleAccount, AccountForListDto>().ConvertUsing<ResponsibleAccountTypeConverter>();
                //.ForMember(dest => dest.Department, opts => opts.MapFrom(src => src.Account.Department))
                //.ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Account.Id))
                //.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Account.Name))
                //.ForMember(dest => dest.AccountRoles, opts => opts.MapFrom(src => src.Account.AccountRoles));
            CreateMap<Measure, MeasureForListDto>();

            CreateMap<IncidentProp, LogDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Account, opts => opts.MapFrom(src => src.Author))
                .ForMember(dest => dest.Action, opts => opts.MapFrom(src => src.Action))
                .ForMember(dest => dest.Info, opts => opts.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Timestamp, opts => opts.MapFrom(src => src.DateCreate));
            CreateMap<ResponsibleProp, LogDto>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Account, opts => opts.MapFrom(src => src.Author))
                .ForMember(dest => dest.Action, opts => opts.MapFrom(src => src.Action))
                .ForMember(dest => dest.Info, opts => opts.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Timestamp, opts => opts.MapFrom(src => src.DateCreate));

            CreateMap<ActivityTypeForCreateDto, ActivityType>();
            CreateMap<BusinessProcessForCreateDto, BusinessProcess>();
            CreateMap<BusinessTypeForCreateDto, BusinessType>();
            CreateMap<RiskAreaForCreateDto, RiskArea>();
            CreateMap<RiskDurationForCreateDto, RiskDuration>();
            CreateMap<RiskFactorForCreateDto, RiskFactor>();
            CreateMap<RiskManageabilityForCreateDto, RiskManageability>();
            CreateMap<RiskReactionForCreateDto, RiskReaction>();
            CreateMap<RiskSignificanceForCreateDto, RiskSignificance>();
            CreateMap<RiskStatusForCreateDto, RiskStatus>();
            CreateMap<TextDataForCreateDto, TextData>();
            CreateMap<IncidentTypeForCreateDto, IncidentType>();

            CreateMap<DraftProp, PropForDetailDto>();

            CreateMap<Log, LogDto>();

            CreateMap<Report, ReportForListDto>();
        }
               
    }

}
