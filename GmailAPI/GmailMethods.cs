using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GmailAPI;

public static class GmailMethods {

    public static List<Gmail> RetrieveSession(DateTime start, DateTime end) {
        List<Gmail> gmails = null;
        GmailService service = GmailMethods.InitializeService();
        string userId = "me";
        string startDate = start.ToString("yyyy/MM/dd");
        string endDate = end.ToString("yyyy/MM/dd");

        // Get a list of emails within the specified date range: API call
        IList<Message> emails = GmailMethods.ListEmails(service, userId, startDate, endDate);
        if (emails == null) {
            Console.WriteLine("Error: API call failed.");
            return gmails;
        }

        // Sort the emails by date in ascending order
        List<Message> sortedEmails = new List<Message>();
        // emails are initially provided in descending order; iterate backwards
        for (int i = emails.Count - 1; i > -1; i--) {
            try {
                // the initial API query via ListRequest.Execute() excludes many fields from the returned
                // Message objects; send a second query via Messages.Get() to retrieve key information
                sortedEmails.Add(service.Users.Messages.Get(userId, emails[i].Id).Execute());
            } catch {
                Console.WriteLine($"API call for Email No. {i} failed.");
            }
        }

        // create a new list of gmail objects and initialize it with specific fields from the email list
        gmails = GmailMethods.CreateGmailList(sortedEmails);
        return gmails;
    }
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
        string CredentialsFilePath = "secrets\\desktop1_secret.json";
        // path to the file that will contain the user's authorization and refresh tokens,
        // constructed in the GWABroker method using your client secret credentials
        string TokenFilePath = "secrets\\";

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

    public static List<Gmail> CreateGmailList(List<Message> sortedEmails) {
        List<Gmail> gmails = new List<Gmail>();
        for (int i = 0; i < sortedEmails.Count; i++) {
            try {
                /*
                Console.WriteLine($"Email No. {i}");
                Console.WriteLine(RetrieveHeader(sortedEmails[i].Payload, "From"));
                string body = "";
                RetrieveBodyWrapper(sortedEmails[i].Payload, "text/plain", ref body);
                Console.WriteLine($"Id: {RetrieveID(sortedEmails[i])}");
                Console.WriteLine($"Internal Date: {RetrieveInternalDate(sortedEmails[i])}");
                DateTime date = UnixTimeStampToDateTime(sortedEmails[i].InternalDate);
                Console.WriteLine(date);
                */
                Gmail newGmail = new Gmail();
                newGmail.Id = RetrieveID(sortedEmails[i]);
                newGmail.InternalDate = RetrieveInternalDate(sortedEmails[i]);
                newGmail.From = RetrieveHeader(sortedEmails[i].Payload, "From");
                newGmail.Subject = RetrieveHeader(sortedEmails[i].Payload, "Subject");
                newGmail.Date = UnixToDate(newGmail.InternalDate);
                // RetrieveBodyWrapper takes body by reference, and sets it to the decoded plain text
                // body of the email
                string body = "";
                RetrieveBodyWrapper(sortedEmails[i].Payload, "text/plain", ref body);
                newGmail.Body = body;
                gmails.Add(newGmail);
            } catch (Exception e) {
                Console.WriteLine($"Error extracting fields for email {i}. {e.Message}");
            }
            //DateTime date = new DateTime(sortedEmails);
        }
        return gmails;
    }

    private static string RetrieveID(Message message) {
        if (message.Id == null) {
            throw new NullReferenceException($"No id found");
        }
        return message.Id;
    }

    private static long RetrieveInternalDate(Message message) {
        if (message.InternalDate == null) {
            throw new NullReferenceException($"No internal date found");
        }
        return message.InternalDate ?? 0;
    }

    private static string RetrieveHeader(MessagePart part, string headerType) {
        List<MessagePartHeader> header = part.Headers.Where(x => x.Name == headerType).ToList();
        if (header.Count > 0) {
            return header[0].Value;
        } else {
            throw new NullReferenceException($"No {headerType} found");
        }
        
    }
    private static DateTime UnixToDate(long ?unixTimeStamp)
    {
        if (unixTimeStamp == null) {
            throw new NullReferenceException("Error: Internal date not found");
        }

        DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp ?? 1);
        DateTime dt = dto.UtcDateTime;
        return dt;
    }

}