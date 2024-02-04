using System.ComponentModel.DataAnnotations;

namespace EmailAssistant.Models;

public class Session
{
    public int Id { get; set; }
    public int SessionNumber { get; set; }
    public string? SessionName { get; set; }
    [DataType(DataType.EmailAddress)]
    public string? EmailAddress { get; set; }
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

}
