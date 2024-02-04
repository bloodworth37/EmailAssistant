using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmailAssistant.Models;

public class Session
{
    
    public int Id { get; set; }
    [DisplayName("Session No.")]
    public int SessionNumber { get; set; }
    [DisplayName("Session Name")]
    public string? SessionName { get; set; }
    [DataType(DataType.EmailAddress)]
    public string? EmailAddress { get; set; }
    [DisplayName("Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    [DisplayName("End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

}
