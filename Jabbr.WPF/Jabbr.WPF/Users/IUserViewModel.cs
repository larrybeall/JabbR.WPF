namespace Jabbr.WPF.Users
{
    public interface IUserViewModel
    {
        bool IsOwner { get; }
        bool IsAway { get; set; }
        bool IsAfk { get; set; }
        string Name { get; set; }
        string Note { get; set; }
        string Gravatar { get; set; }
        GroupType Group { get; set; }
    }
}