namespace GmailAPI {
    public class GmailSender {
        public int SessionNumber { get; set; }
        public string? SessionEmailAddress { get; set; }
        public string? SenderAddress { get; set; }
        public int NumEmails { get; set; }

        public GmailSender(int sessionNumber, string sessionEmailAddress, string senderAddress, int numEmails) {
            SessionNumber = sessionNumber;
            SessionEmailAddress = sessionEmailAddress;
            SenderAddress = senderAddress;
            NumEmails = numEmails;
        }

    }
}