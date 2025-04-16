using RestauranteService.Dtos;

namespace RestauranteService.ItemServiceHttpClient;

public interface IItemServiceHttpClient
{
    public Task EnviaRestauranteParaItemServiceAsync(RestauranteReadDto readDto);
}
