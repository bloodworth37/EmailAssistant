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

    public static UserCredential GetCredentials() {
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

    public static IList<Message> ListEmails(GmailService service, string userId, string startDate, string endDate) {
        try
        { 
            var listRequest = service.Users.Messages.List(userId);
            // Construct the query to filter emails within the specified date range
            string query = $"after:{startDate} before:{endDate}";
            listRequest.Q = query;

            // Execute the request; retrieve the emails from the API
            return listRequest.Execute().Messages;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    private static void RetrieveBodyWrapper(MessagePart part, string bodyType, ref string plaintext) {
        RetrieveBody(part, bodyType, ref plaintext);
        if (plaintext == null || plaintext == "") {
            throw new NullReferenceException($"No body found.");
        }
        byte[] bodyBytes = Base64ToUrl(plaintext);
        string decodedBody = System.Text.Encoding.UTF8.GetString(bodyBytes);
        plaintext = decodedBody;
    }

    // check to confirm the assumption that only one text/plain MIME part occurs in each email body
    private static void RetrieveBody(MessagePart part, string bodyType, ref string plaintext) {
        //Console.WriteLine(part.MimeType);
        if (part.MimeType == bodyType) {
            plaintext = part.Body.Data;
        }
        if (part.Parts != null) {
            if (part.Parts.Count > 0) {
                foreach (var newPart in part.Parts) {
                    RetrieveBody(newPart, bodyType, ref plaintext);
                }
            }
        }
    }

    public static byte[] Base64ToUrl(string base64Url) {
        string padded = base64Url.Length % 4 == 0
            ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
        string base64 = padded.Replace("_", "/")
                                .Replace("-", "+");
        return Convert.FromBase64String(base64);
    }

}