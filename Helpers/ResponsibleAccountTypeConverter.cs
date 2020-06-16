using AutoMapper;
using sors.Authentication;
using sors.Data.Dto;
using sors.Data.Entities;
using sors.Data.Entities.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Helpers
{
    public class ResponsibleAccountTypeConverter : ITypeConverter<ResponsibleAccount, AccountForListDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public ResponsibleAccountTypeConverter(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }
        public AccountForListDto Convert(ResponsibleAccount source, AccountForListDto destination, ResolutionContext context)
        {
            destination = new AccountForListDto();
            destination.Id = source.Account.Id;
            destination.Name = source.Account.Name;
            destination.AccountRoles = _mapper.Map<IEnumerable<RoleForListDto>>(source.Account.AccountRoles);
            destination.Department = _mapper.Map<DepartmentForListDto>(source.Account.Department);
            destination = _userService.FillAccountInfo(destination);
            return destination;
        }
    }
}
