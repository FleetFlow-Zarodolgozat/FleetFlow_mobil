namespace mobil.Services
{
    public enum ThemePreference
    {
        System = 0,
        Light = 1,
        Dark = 2
    }

    public class ThemeService
    {
        private const string ThemeKey = "app_theme";

        public ThemePreference CurrentTheme
        {
            get => (ThemePreference)Preferences.Get(ThemeKey, (int)ThemePreference.System);
            set
            {
                Preferences.Set(ThemeKey, (int)value);
                ApplyTheme(value);
            }
        }

        public void ApplyTheme(ThemePreference theme)
        {
            if (Application.Current is null) return;
            Application.Current.UserAppTheme = theme switch
            {
                ThemePreference.Light => AppTheme.Light,
                ThemePreference.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }

        public void ApplySavedTheme()
        {
            ApplyTheme(CurrentTheme);
        }
    }
}
