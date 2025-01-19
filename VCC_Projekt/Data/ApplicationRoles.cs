using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace VCC_Projekt.Data
{
        public class ApplicationRole : IdentityRole
        {
            public override string Id { get => base.Name; set => base.Name = value; }
            public new string Beschreibung {  get; set; }
        }



}
