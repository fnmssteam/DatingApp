namespace API.Extensions;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;
        
        // if the bday hasn't occured yet this year.
        if (dob > today.AddYears(-age)) age--;

        return age;
    }
}
