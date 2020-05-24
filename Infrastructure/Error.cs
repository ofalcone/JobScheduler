using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobScheduler.Infrastructure
{
    public static class Error
    {
        public static IdentityException Identity(IdentityError error)
        {
            return new IdentityException(error.Code, error.Description);
        }

        public static AggregateException Identity(IEnumerable<IdentityError> errors)
        {
            return new AggregateException(errors.Select(Identity));
        }
    }
}
