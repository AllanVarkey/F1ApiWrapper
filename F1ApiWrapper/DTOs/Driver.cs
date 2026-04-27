namespace F1ApiWrapper.DTOs;

public class Driver
{
    public int DriverNumber { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string NameAcronym { get; set; } = string.Empty;

    public string Team { get; set; } = string.Empty;
}
