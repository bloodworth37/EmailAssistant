using System.ComponentModel.DataAnnotations;

namespace EmailAssistant.Models;

public class Email
{
    public int Id { get; set; }

    public string? EmailId { get; set; }
    public long InternalDate { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime Date { get; set; }
    public string? Body { get; set; }
    [DataType(DataType.EmailAddress)]
    public string? From { get; set; }
    public string? Subject { get; set; }
    public int SessionNumber { get; set; }

}
