namespace NHSv2.Appointments.Domain.Appointments;

public class Appointment
{
    public Guid Id { get; private init; }

    public string Test { get; set; }

    public Appointment(Guid id, string test)
    {
        Id = id;
        Test = test;
    }
}