using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoTrackApp.src.services
{
    public interface IUserServices
    {
        /*
        Returns a AppResponse with status 200 if the login was successful,
        or 40x if there is a problem with the login.
        */
        /// <summary>
        /// Login an user if it's registered in the database.
        /// </summary>
        /// <param name="pPassword">User's password to login.</param>
        /// <param name="pEmail">User's email password to login.</param>
        /// <returns>
        /// A string with the user's Id if the loggin is success.<br>
        /// Null if there is not user with the given email.
        /// </returns>
        /// <exception cref="InvalidOperationException"> If the passwords doesn't match. </exception>
        /// <exception cref="Exception">If there has been an unexpected error while login.</exception>
        Task<string?> LoginUser(String pPassword, String pEmail);
	    Task AddUserAsync(string pEmail, string pPassword, string pUserName, DateTime pBirthDate);
	    Task<bool> IsEmailAvailable(string pEmail);
    }
}

