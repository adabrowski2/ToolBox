using Praca_Inżynierska_v1.MVVM.Model;

namespace Praca_Inżynierska_v1.Services.Client_Service
{
    public interface IClientService
    {
        Task<List<Client>> GetAllClientsAsync();

        Task UpdateClientsAsync(IEnumerable<Client> clients);

        Task AddClientAsync(Client client);
    }
}
