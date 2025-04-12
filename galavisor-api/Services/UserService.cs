using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;
public class UserService(UserRepository repo)
{
    private readonly UserRepository _userRepository = repo;

    public async Task<UserModel?> GetUser(string sub)
    {
        return await _userRepository.GetBySub(sub);
    }
}
