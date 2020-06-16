using AutoMapper;
using sors.Authentication;
using sors.Data.Dto;
using sors.Data.Dto.Incidents;
using sors.Data.Entities;
using sors.Data.Entities.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Helpers
{
    public class IncidentDraftTypeConverter : ITypeConverter<IncidentDraft, DraftForDetailDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public IncidentDraftTypeConverter(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }
        public DraftForDetailDto Convert(IncidentDraft source, DraftForDetailDto destination, ResolutionContext context)
        {
            destination = _mapper.Map<DraftForDetailDto>(source.Draft);
            return destination;
        }
    }
}
