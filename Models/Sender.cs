using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmailAssistant.Models;

public class Sender
{

    public int Id { get; set; }
    [DisplayName("Session No.")]
    public int SessionNumber { get; set; }
    [DataType(DataType.EmailAddress)]
    public string? SessionEmailAddress { get; set; }
    public string? SenderAddress { get; set; }
    public int NumEmails { get; set; }

    public Sender() {}

    public Sender(int sessionNumber, string sessionEmailAddress, string senderAddress, int numEmails) {
        SessionNumber = sessionNumber;
        SessionEmailAddress = sessionEmailAddress;
        SenderAddress = senderAddress;
        NumEmails = numEmails;
    }

}
