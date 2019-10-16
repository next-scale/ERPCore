using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Models.Profiles
{
    public enum ProfileType
    {
        Organization = 0,
        People = 1
    }

    public enum ProfileViewType
    {
        Organization = 0,
        People = 1,
        Active = 11,
        InActive = 12
    }
}
