using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Error
{
    public enum EnumErrors
    {
        MYSQL_ERROR,
        NULL_REFERENCE,

        NOT_FOUND,
        ACCESS_DENIED,

        SESSION_EXPIRED,
        BAD_REQUEST,

        EXTERNAL_RESPONSE_ERROR,

        UNKNOWN_ERROR,
    }
}
