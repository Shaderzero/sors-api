using AutoMapper;
using sors.Authentication;
using sors.Data.Dto;
using sors.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sors.Helpers
{
    public class AccountTypeConverter : ITypeConverter<Account, AccountForListDto>
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public AccountTypeConverter(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }
        public AccountForListDto Convert(Account source, AccountForListDto destination, ResolutionContext context)
        {
            destination = new AccountForListDto();
            destination.Id = source.Id;
            destination.Name = source.Name;
            destination.AccountRoles = _mapper.Map<IEnumerable<RoleForListDto>>(source.AccountRoles);
            destination.Department = _mapper.Map<DepartmentForListDto>(source.Department);
            //destination.Fullname = _userService.RetrieveFullname(source.Name);
            //destination.Email = _userService.RetrieveEmail(source.Name);
            //destination.Fullname = _userService.GetFullname(source.Name);
            destination = _userService.FillAccountInfo(destination);
            return destination;
        }
    }
}
