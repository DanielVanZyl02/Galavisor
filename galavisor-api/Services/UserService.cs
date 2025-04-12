using GalavisorApi.Models;
using GalavisorApi.Repositories;

namespace GalavisorApi.Services;
public class UserService(UserRepository repo)
{
    private readonly UserRepository _userRepository = repo;

    public async Task<List<UserModel>> GetAllUsers()
    {
        return await _userRepository.GetAll();
    }

    public async Task<UserModel?> GetUser(int Id)
    {
        return await _userRepository.GetById(Id);
    }
}
