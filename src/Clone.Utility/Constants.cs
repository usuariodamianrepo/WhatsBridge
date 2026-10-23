namespace Clone.Utility
{
    public static class Constants
    {
        public const int ItemsMaxNumber = 100;
        public const int ItemsPerPage = 10;
        public const int KeyCodeEnter = 13;
        public const string KeyEnter = "Enter";
        public const string OptionEmpty = "-- Empty --";
        public const string TabContent = "tab-content";
        public const string TabDetails = "tab-details";
        public const string TabList = "tab-list";
        public const int MaxFifty = 50;
        public const int MaxOneHundred = 100;
        public const int MaxThirty = 30;
    }

    /// <summary>
    /// IANA funciona nativo en .NET Core en Linux/Docker
    /// </summary>
    public static class Iana
    {
        public const string MyZone = "America/Argentina/Buenos_Aires";
    }

    public static class MaxRows
    {
        public const int MaxFifty = 50;
        public const int MaxOneHundred = 100;
        public const int MaxThirty = 30;
    }
    /// <summary>
    /// Role Definitions (RD) constants for roles in Spanish.
    /// </summary>
    public static class RD
    {
        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Cliente";
        public const string RoleEmployee = "Empleado";
        public const string RoleSupplier = "Proveedor";
    }
}
