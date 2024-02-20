using System;
using System.Collections;
using System.Collections.Generic;
using Pango;

public struct AppResponse{
    public int status;
    public IDictionary data;
}

namespace CryptoTrackApp.src.services
{
    public interface IUserServices
    {
        /*
        Returns a AppResponse with status 200 if the login was successful,
        or 40x if there is a problem with the login.
        */
        AppResponse LoginUser(String pPassword, String pEmail);
    }
}
