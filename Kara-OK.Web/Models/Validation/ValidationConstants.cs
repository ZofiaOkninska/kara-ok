namespace Kara_OK.Web.Models.Validation;

public static class ValidationConstants
{
    public const int RoomNameMaxLength = 80;
    public const int EquipmentDescriptionMaxLength = 500;

    public const int RoomMinCapacity = 1;
    public const int RoomMaxCapacity = 500;

    public const decimal RoomMinPricePerHour = 0m;
    public const decimal RoomMaxPricePerHour = 10000m;

    // Maximum booking duration is 6 hours = 360 minutes
    // Entity checks
    public const int BookingMinDurationMinutes = 15;
    public const int BookingMaxDurationMinutes = 360; 
    // Create ViewModel checks
    public const int BookingMinDurationExtraMinutes = 0;
    public const int BookingMaxDurationExtraMinutes = 45;
    public const int BookingMinHours = 0;
    public const int BookingMaxHours = 6;

    public const int BookingMinPeopleCount = 1;
    public const int BookingMaxPeopleCount = 500;
}