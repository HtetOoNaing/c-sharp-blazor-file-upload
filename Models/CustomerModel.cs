namespace BlazorDemo.Models;

public class CustomerModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = "admin";
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string>? FileNames { get; set; } = [];
}