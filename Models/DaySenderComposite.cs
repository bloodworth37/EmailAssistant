namespace EmailAssistant.Models;

public class DaySenderComposite {

    public IEnumerable<Day>? Days { get; set; }
    
    public IEnumerable<Sender>? Senders { get; set; }

    public DaySenderComposite(IEnumerable<Day> days, IEnumerable<Sender> senders) {
        Days = days;
        Senders = senders;
    }

}