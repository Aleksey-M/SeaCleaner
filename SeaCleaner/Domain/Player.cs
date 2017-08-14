using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SeaCleaner.Domain
{
    public class Player : IdentityUser
    {        
        public IList<GameResults> GameResults { get; set; }
    }
}
