namespace Tenon.AspNetCore.Authentication.Google.Extensions.Configurations;

/*
 * A few notes on loggin in with Google:
   Google will only return a refresh token if you specify access_type=offline in your auth request.
   Google will only return a refresh token on the user's first authorization request, unless you always specify prompt=consent in your query params.
   In my experience, when leaving out the prompt query param, the user is not prompted for their consent again. If they are logged in to google, you will get a new access token, but no refresh token, unless you have prompt=consent.
   I think the idea is you use prompt=consent if you have no record of the user ever using your application. Otherwise if they have used it before, you may prefer to use prompt=select_account to allow the user to select which account he wants to use in case he has more then one, or you can just use prompt=none.
   https://stackoverflow.com/questions/44078190/google-oauth-always-showing-consent-screen
 */
internal sealed class GoogleOptions : Microsoft.AspNetCore.Authentication.Google.GoogleOptions
{
    /// <summary>
    ///     none,
    ///     consent,
    ///     select_account
    /// </summary>
    public string Prompt { get; set; } = "none";
}