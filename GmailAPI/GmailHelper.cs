using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GmailAPI;

public static class GmailMethods {

    public static GmailService InitializeService() {
        string ApplicationName = "Email Assistant";
        // the credential object needed to construct the GmailService object
        UserCredential credential = GetCredentials();

        // Create Gmail API service
        return new GmailService(new BaseClientService.Initializer()
        {
            // provides the previously constructed credential to authenticate the GmailService
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public static UserCredential GetCredentials()
        {
            string[] Scopes = { GmailService.Scope.GmailReadonly };
            // path to the client_secret file; used to construct the file stream
            string CredentialsFilePath = "C:\\GmailAPI\\ClientCredentials\\desktop1_secret.json";
            // path to the file that will contain the user's authorization and refresh tokens,
            // constructed in the GWABroker method using your client secret credentials
            string TokenFilePath = "C:\\GmailAPI\\CredentialsInfo";

            // create FileStream to read client secret file
            using (var stream = new FileStream(CredentialsFilePath, FileMode.Open, FileAccess.Read))
            {
                // Authorize the user; create token.json, or access existing token.json
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    // initialize a GoogleClientSecrets object with the FromStream constructor
                    // load the variables in the client secret file into the new object, and access them
                    // via the Secrets property
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    // the list of scopes enabled for this credential instance
                    Scopes,
                    "user",
                    CancellationToken.None,
                    // creates a new file containing the user's access and refresh tokens, placing it at
                    // the specified tokenfilepath
                    new FileDataStore(TokenFilePath, true)).Result;
            }
        }

}