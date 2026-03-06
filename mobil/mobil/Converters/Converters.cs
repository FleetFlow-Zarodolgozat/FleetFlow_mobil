using System.Globalization;

namespace mobil.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }

    public class PasswordIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isVisible && isVisible)
                return "\uf06e";
            return "\uf070";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is not null;
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class IsNotZeroConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is int count && count > 0;
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? Color.FromArgb("#f1f5f9") : Color.FromArgb("#1173d4");
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? Color.FromArgb("#64748b") : Colors.White;
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class NotificationTypeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "ACCOUNT"         => Color.FromArgb("#1173d4"),
                "ASSIGNMENT"      => Color.FromArgb("#7c3aed"),
                "FUEL_LOG"        => Color.FromArgb("#EA580C"),
                "SERVICE_REQUEST" => Color.FromArgb("#059669"),
                "TRIP"            => Color.FromArgb("#0891b2"),
                _                 => Color.FromArgb("#64748b"),
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class NotificationTypeToLightColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "ACCOUNT"         => Color.FromArgb("#F0F7FF"),
                "ASSIGNMENT"      => Color.FromArgb("#f5f3ff"),
                "FUEL_LOG"        => Color.FromArgb("#FFF5E5"),
                "SERVICE_REQUEST" => Color.FromArgb("#ecfdf5"),
                "TRIP"            => Color.FromArgb("#ecfeff"),
                _                 => Color.FromArgb("#f1f5f9"),
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class NotificationTypeToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "ACCOUNT"         => "\uf007",
                "ASSIGNMENT"      => "\uf0d1",
                "FUEL_LOG"        => "\uf52f",
                "SERVICE_REQUEST" => "\uf0ad",
                "TRIP"            => "\uf14e",
                _                 => "\uf0f3",
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class NotificationTypeToLabelConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "ACCOUNT"         => "Account",
                "ASSIGNMENT"      => "Assignment",
                "FUEL_LOG"        => "Fuel Log",
                "SERVICE_REQUEST" => "Service",
                "TRIP"            => "Trip",
                _                 => "Notification",
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class TabColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) == (parameter as string)
                ? Color.FromArgb("#1173d4")
                : Color.FromArgb("#94a3b8");
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class TabFontAttributesConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) == (parameter as string)
                ? FontAttributes.Bold
                : FontAttributes.None;
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ServiceStatusToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "REQUESTED"   => Color.FromArgb("#1173d4"),
                "REJECTED"    => Color.FromArgb("#dc2626"),
                "APPROVED"    => Color.FromArgb("#059669"),
                "DRIVER_COST" => Color.FromArgb("#EA580C"),
                "CLOSED"      => Color.FromArgb("#64748b"),
                _             => Color.FromArgb("#94a3b8"),
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ServiceStatusToLightColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "REQUESTED"   => Color.FromArgb("#F0F7FF"),
                "REJECTED"    => Color.FromArgb("#fef2f2"),
                "APPROVED"    => Color.FromArgb("#ecfdf5"),
                "DRIVER_COST" => Color.FromArgb("#FFF5E5"),
                "CLOSED"      => Color.FromArgb("#f1f5f9"),
                _             => Color.FromArgb("#f1f5f9"),
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ServiceStatusToLabelConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "REQUESTED"   => "Requested",
                "REJECTED"    => "Rejected",
                "APPROVED"    => "Approved",
                "DRIVER_COST" => "Awaiting Cost",
                "CLOSED"      => "Closed",
                _             => "Unknown",
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ServiceStatusToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => (value as string) switch
            {
                "REQUESTED"   => "\uf017",
                "REJECTED"    => "\uf00d",
                "APPROVED"    => "\uf00c",
                "DRIVER_COST" => "\uf155",
                "CLOSED"      => "\uf023",
                _             => "\uf128",
            };
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}