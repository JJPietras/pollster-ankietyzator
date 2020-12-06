namespace Ankietyzator.Models.DataModel.AccountModel
{
    public enum UserType
    {
        User,
        Pollster,
        Admin
    }

    public static class Extensions
    {
        public static string GetRole(this UserType userType)
        {
            switch (userType)
            {
                case UserType.User: return "user";
                case UserType.Pollster: return "pollster";
                default: return "admin";
            }
        }
    }
}