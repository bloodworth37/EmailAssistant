namespace EmailAssistant.Models;

public class BigText
{
    public int Id { get; set; }
    public string? Body { get; set; }

    public BigText(string body) {
        Body = body;
    }

}