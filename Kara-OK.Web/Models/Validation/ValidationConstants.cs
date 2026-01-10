namespace Kara_OK.Web.Models.Validation;

public static class ValidationConstants
{
    public const int RoomNameMaxLength = 80;
    public const int EquipmentDescriptionMaxLength = 500;

    public const int RoomMinCapacity = 1;
    public const int RoomMaxCapacity = 500;

    public const decimal RoomMinPricePerHour = 0m;
    public const decimal RoomMaxPricePerHour = 10000m;

    public const int BookingMinDurationMinutes = 15;
    public const int BookingMaxDurationMinutes = 360; // Maximum booking duration is 6 hours

    public const int BookingMinPeopleCount = 1;
    public const int BookingMaxPeopleCount = 500;
}