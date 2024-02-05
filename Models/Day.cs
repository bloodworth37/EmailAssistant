using System.ComponentModel.DataAnnotations;

namespace EmailAssistant.Models;

public class Day
{
    public int Id { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime Date { get; set; }
    public int NumEmails { get; set; }

    public Day(DateTime date, int numEmails) {
        Date = date;
        NumEmails = numEmails;
    }

}
