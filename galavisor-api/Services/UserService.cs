using System.ComponentModel;
using System.Threading.Tasks;
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

    public async Task<UserModel?> GetUser(string sub)
    {
        return await _userRepository.GetBySub(sub);
    }

    public async Task<UserModel> UpdateUserConfig(string GoogleSubject, string PlanetName, string NewUserName){
        return await _userRepository.UpdateUserConfig(GoogleSubject, PlanetName, NewUserName);
    }

    public async Task<UserModel> UpdateActiveStatusBySub(bool IsActive, string GoogleSubject){
        return await _userRepository.UpdateActiveStatusBySub(GoogleSubject, IsActive);
    }

    public async Task<UserModel> UpdateActiveStatusById(bool IsActive, int UserId){
        return await _userRepository.UpdateActiveStatusById(UserId, IsActive);
    }

    public async Task<UserModel> UpdateRole(string Role, int UserId){
        return await _userRepository.UpdateRole(UserId, Role);
    }
}
