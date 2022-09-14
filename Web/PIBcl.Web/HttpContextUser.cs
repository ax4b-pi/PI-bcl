using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DuCorp.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace DuCorp.Web
{
    public class HttpContextUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //[JsonProperty(PropertyName = "id")]
        //public Guid Id => new Guid(_httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value); => "";

        //[JsonProperty(PropertyName ="name")]
        //public string Name => _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.GivenName).Value;
        
        [JsonIgnore]
        public IEnumerable<Claim> Claims => _httpContextAccessor.HttpContext.User.Claims;

        private Guid id = Guid.Empty;

        [JsonProperty(PropertyName = "id")]
        public Guid Id {

            get {

                Claim claim = null;

                if(_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                    return id;

                claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "extension_UserId");
                
                if (null == claim)
                {
                    try
                    {
                        claim = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
                    }
                    catch { }
                }
                
                if (null == claim)
                    return id;

                id = new Guid(claim.Value);

                return id;
            }
            private set 
            {
                id = value;
            }
        }


        public string Email { get; set; }

        public string Telefone { get; set; }


        private string name = "";
        [JsonProperty(PropertyName = "name")]
        public string Name {
            get {

                try
                {
                    name = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.GivenName).Value;
                }
                catch{}
                
                return name;

            }
            private set {
                name = value;
            }
        }     
    }
}
