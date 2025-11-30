namespace ATM.Interfaces
{
    public interface IJsonStorageService
    {
        List<User> LoadUsers();
        void SaveUsers(List<User> users);
    }

}
