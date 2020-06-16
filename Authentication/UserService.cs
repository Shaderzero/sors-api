using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sors.Data;
using sors.Data.Dto;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sors.Authentication
{
    public class UserService : IUserService
    { 
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private List<DomainUser> _domainUsers;
        private DateTime lastUpdate;

        public UserService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }
        
        public void UpdateDomainUsers()
        {
            var myDomainUsers = new List<DomainUser>();
            using (var ctx = new PrincipalContext(ContextType.Domain, _configuration["Domain"]))
            {
                var userPrincipal = new UserPrincipal(ctx);
                userPrincipal.Enabled = true;
                using (var search = new PrincipalSearcher(userPrincipal))
                {
                    foreach (UserPrincipal domainUser in search.FindAll())
                    {
                        if (domainUser.SamAccountName != null)
                        {
                            DirectoryEntry dirEntry = (DirectoryEntry)domainUser.GetUnderlyingObject();
                            if (dirEntry.Properties["Department"].Value != null)
                            {
                                string eMail = domainUser.EmailAddress;
                                string userDepartment = dirEntry.Properties["Department"].Value.ToString();
                                DomainUser dUser = new DomainUser
                                {
                                    Name = domainUser.SamAccountName,
                                    Fullname = domainUser.DisplayName,
                                    DomainDepartment = userDepartment,
                                    Email = eMail
                                };
                                myDomainUsers.Add(dUser);
                            }
                        }
                    }
                }
            }
            _domainUsers = myDomainUsers;
            lastUpdate = DateTime.UtcNow;
        }

        public List<DomainUser> GetDomainUsers()
        {
            var timeCompare = DateTime.Compare(DateTime.UtcNow, lastUpdate.AddDays(1));
            if (_domainUsers == null || timeCompare > 0)
            {
                UpdateDomainUsers();
            }
            return _domainUsers;
        }

        public string GetFullname(string name)
        {
            var domainUsers = GetDomainUsers();
            var existUser = domainUsers.Where(d => d.Name == name).FirstOrDefault();
            if (existUser == null)
                return name;
            return existUser.Fullname;
            //using (var context = new PrincipalContext(ContextType.Domain))
            //{
            //    var usr = UserPrincipal.FindByIdentity(context, name);
            //    if (usr != null)
            //    {
            //        return usr.DisplayName;
            //    } else
            //    {
            //        return name;
            //    }
            //}
        }

        public string GetEmail(string name)
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var usr = UserPrincipal.FindByIdentity(context, name);
                if (usr != null)
                {
                    return usr.EmailAddress;
                }
                else
                {
                    return "none";
                }
            }
        }

        public string GetDep(string name)
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var usr = UserPrincipal.FindByIdentity(context, name);
                if (usr != null)
                {
                    DirectoryEntry dirEntry = (DirectoryEntry)usr.GetUnderlyingObject();
                    if (dirEntry.Properties["Department"].Value != null)
                        return dirEntry.Properties["Department"].Value.ToString();
                    else return "none";
                }
                else
                {
                    return "none";
                }
            }
        }

        public AccountForListDto FillAccountInfo(AccountForListDto source)
        {
            var domainUsers = GetDomainUsers();
            var existUser = domainUsers.Where(d => d.Name == source.Name).FirstOrDefault();
            if (existUser == null)
            {
                source.Fullname = source.Name;
                source.Email = source.Name + "@gazpromexport.gazprom.ru";
            }
            else
            {
                source.Fullname = existUser.Fullname;
                source.Email = existUser.Email;
            }
            return source;
        }

        public AccountForListDto FillAccountInfoOLD(AccountForListDto source)
        {
            string name = source.Name;
            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var usr = UserPrincipal.FindByIdentity(context, name);
                if (usr != null)
                {
                    source.Fullname = usr.DisplayName;
                    source.Email = usr.EmailAddress;
                    DirectoryEntry dirEntry = (DirectoryEntry)usr.GetUnderlyingObject();
                    //if (dirEntry.Properties["Department"].Value != null)
                    //    source.DomainDepartment = dirEntry.Properties["Department"].Value.ToString();
                    //else source.DomainDepartment = "none";
                }
                else
                {
                    source.Fullname = "none";
                    source.Email = "none";
                    //source.DomainDepartment = "none";
                }
                return source;
            }
        }

    }
}
